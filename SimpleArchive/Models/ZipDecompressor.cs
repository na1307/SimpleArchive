using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Common.Zip;
using SharpCompress.Readers;
using SharpCompress.Readers.Zip;
using SimpleArchive.ViewModels;
using SimpleArchive.Views;

namespace SimpleArchive.Models;

internal sealed partial class ZipDecompressor : IDecompressor {
    private readonly string archivePath;
    private readonly string destinationPath;
    private readonly CancellationTokenSource cts;
    private readonly ManualResetEventSlim mres;
    private readonly ProgressWindowViewModel pwvm;
    private readonly ProgressState state;
    private bool startedOnce;

    public ZipDecompressor(string archivePath, string destinationPath, bool extractSmart) {
        this.archivePath = archivePath;
        this.destinationPath = IsExtractToSubDirectory(extractSmart, archivePath) ? GetSubDir(archivePath, destinationPath) : destinationPath;
        cts = new();
        mres = new(true);
        pwvm = new(cts, mres);
        state = new();
        ProgressWindow = ProgressWindow.Create(pwvm);
    }

    public ProgressWindow ProgressWindow { get; }

    public Task DecompressAsync() {
        if (startedOnce) {
            throw new InvalidOperationException("The decompressor has already been started and can't be reuse.");
        }

        var updaterTask = RunUpdaterTask();
        var decompressorTask = RunDecompressorTask();
        startedOnce = true;

        return Task.WhenAll(updaterTask, decompressorTask);
    }

    public void Dispose() {
        cts.Dispose();
        mres.Dispose();
    }

    public ValueTask DisposeAsync() {
        cts.Dispose();
        mres.Dispose();

        return ValueTask.CompletedTask;
    }

    private static bool IsExtractToSubDirectory(bool extractSmart, string archivePath) {
        if (!extractSmart) {
            return false;
        }

        using var tempFs = File.OpenRead(archivePath);
        using var tempArchive = ZipArchive.Open(tempFs);

        var directories = tempArchive.Entries.Where(e => e.IsDirectory).ToArray();
        var files = tempArchive.Entries.Where(e => !e.IsDirectory).ToArray();
        var firstDirectory = directories.FirstOrDefault();

        return !((firstDirectory is null && files.Length == 1)
            || (firstDirectory is not null && tempArchive.Entries.All(e => e.Key!.StartsWith(firstDirectory.Key!))));
    }

    private static string GetSubDir(string path, string dest) {
        var path2 = Path.GetFileNameWithoutExtension(path);
        var i = 0;
        string destPath;

        do {
            destPath = Path.Combine(dest, ++i != 1 ? $"{path2} ({i})" : path2);
        } while (Directory.Exists(destPath));

        return destPath;
    }

    private static IEnumerable<ZipEntry> GetEntries(ZipReader reader) {
        while (reader.MoveToNextEntry()) {
            yield return reader.Entry;
        }
    }

    private async Task RunUpdaterTask() {
        try {
            using PeriodicTimer pt = new(TimeSpan.FromMilliseconds(100));

            while (await pt.WaitForNextTickAsync(cts.Token).ConfigureAwait(false)) {
                ProgressWindow.DispatcherQueue.TryEnqueue(() => {
                    pwvm.MaxOverall = state.OverallMax;
                    pwvm.OverallValue = state.OverallValue;
                    pwvm.CurrentPath = state.CurrentPath;
                    pwvm.MaxCurrent = state.CurrentMax;
                    pwvm.CurrentValue = state.CurrentValue;
                });
            }
        } catch (OperationCanceledException) {
            // Ignore
        }
    }

    private async Task RunDecompressorTask() {
        try {
            var password = await ResolveArchivePassword();

            if (password is null) {
                return;
            }

            await using FileStream archiveFileStream = new(archivePath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true);
            state.OverallMax = archiveFileStream.Length;
            await using ReadProgressStream rfs = new(archiveFileStream, p => state.OverallValue = p);

            using var reader = ZipReader.Open(rfs, new() {
                Password = password
            });

            bool? rememberOverwrite = null;

            if (!Directory.Exists(destinationPath)) {
                Directory.CreateDirectory(destinationPath);
            }

            foreach (var entry in GetEntries(reader)) {
                if (cts.IsCancellationRequested) {
                    return;
                }

                var entryDest = Path.Combine(destinationPath, entry.Key!);

                if (entry.IsDirectory) {
                    Directory.CreateDirectory(entryDest);

                    continue;
                }

                state.CurrentPath = entry.Key;
                state.CurrentMax = entry.Size;
                state.CurrentValue = 0L;

                if (File.Exists(entryDest)) {
                    bool overwrite;

                    if (rememberOverwrite is null) {
                        (overwrite, rememberOverwrite) = await showFileExistsDialog(entryDest);
                    } else {
                        overwrite = rememberOverwrite.Value;
                    }

                    if (!overwrite) {
                        continue;
                    }
                }

                try {
                    await using FileStream efs = new(entryDest, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);

                    await using (WriteProgressStream wefs = new(efs, p => state.CurrentValue = p, true))
                    await using (var es = reader.OpenEntryStream())
                    await using (SuspendableReadStream srs = new(es, mres)) {
                        await srs.CopyToAsync(wefs, cts.Token);
                    }

                    if (entry.LastModifiedTime is not null) {
                        File.SetLastWriteTime(efs.SafeFileHandle, entry.LastModifiedTime.Value);
                    }
                } catch (OperationCanceledException) {
                    File.Delete(entryDest);

                    return;
                }

                state.CurrentValue = entry.Size;
            }

            state.OverallValue = archiveFileStream.Length;
        } finally {
            if (!cts.IsCancellationRequested) {
                await cts.CancelAsync();
            }
        }
    }

    private async Task<string?> ResolveArchivePassword() {
        var password = string.Empty;
        var passwordResolved = false;
        await using var tempFs = File.OpenRead(archivePath);

        var tempReader = ReaderFactory.Open(tempFs, new() {
            LeaveStreamOpen = true
        });

        try {
            do {
                try {
                    tempReader.MoveToNextEntry();
                    passwordResolved = true;
                } catch (Exception e) when (e is CryptographicException or InvalidFormatException) {
                    tempReader.Dispose();

                    PasswordControlViewModel pcvm = new();

                    if (await showPasswordDialog(pcvm)) {
                        password = pcvm.Password;
                        tempFs.Position = 0;

                        tempReader = ReaderFactory.Open(tempFs, new() {
                            LeaveStreamOpen = true,
                            Password = password
                        });
                    } else {
                        return null;
                    }
                }
            } while (!passwordResolved);
        } finally {
            tempReader.Dispose();
        }

        return password;
    }

    private Task<bool> showPasswordDialog(PasswordControlViewModel pcvm) {
        TaskCompletionSource<bool> tcs = new();

        ProgressWindow.DispatcherQueue.TryEnqueue(async () => {
            while (ProgressWindow.Content.XamlRoot is null) {
                await Task.Delay(50);
            }

            ContentDialog cd = new() {
                XamlRoot = ProgressWindow.Content.XamlRoot,
                Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"],
                Title = "Password required",
                Content = new PasswordControl(pcvm),
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary
            };

            try {
                var value = await cd.ShowAsync() == ContentDialogResult.Primary;

                tcs.TrySetResult(value);
            } catch (Exception e) {
                tcs.TrySetException(e);
            }
        });

        return tcs.Task;
    }

    private Task<(bool Overwrite, bool? RememberOverwrite)> showFileExistsDialog(string dest) {
        TaskCompletionSource<(bool, bool?)> tcs = new();

        ProgressWindow.DispatcherQueue.TryEnqueue(async () => {
            while (ProgressWindow.Content.XamlRoot is null) {
                await Task.Delay(50);
            }

            FileExistsControlViewModel fecvm = new(dest);

            ContentDialog cd = new() {
                XamlRoot = ProgressWindow.Content.XamlRoot,
                Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"],
                Title = "File already exists",
                Content = new FileExistsControl(fecvm),
                PrimaryButtonText = "Overwrite",
                SecondaryButtonText = "Skip",
                DefaultButton = ContentDialogButton.Secondary
            };

            try {
                var value = await cd.ShowAsync() == ContentDialogResult.Primary;
                bool? remember = fecvm.Remember ? value : null;

                tcs.TrySetResult((value, remember));
            } catch (Exception e) {
                tcs.TrySetException(e);
            }
        });

        return tcs.Task;
    }
}
