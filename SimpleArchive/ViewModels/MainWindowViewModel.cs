using CommunityToolkit.Mvvm.Input;
using Microsoft.Windows.Storage.Pickers;
using SharpCompress.Readers;
using SimpleArchive.Models;
using SimpleArchive.Views;
using WinUIEx;

namespace SimpleArchive.ViewModels;

internal sealed partial class MainWindowViewModel : ViewModel {
    private string? openedFile;

    private static async Task RunUpdaterTask(ProgressWindow pw, ProgressWindowViewModel pwvm, ProgressState state) {
        while (!state.IsCompleted) {
            await Task.Delay(100);

            pw.DispatcherQueue.TryEnqueue(() => {
                pwvm.MaxArchive = state.OverallMax;
                pwvm.ArchiveValue = state.OverallValue;
                pwvm.CurrentPath = state.CurrentPath;
                pwvm.MaxCurrent = state.CurrentMax;
                pwvm.CurrentValue = state.CurrentValue;
            });
        }
    }

    private static async Task RunDecompressorTask(string openedFile, string destinationPath, ProgressState state) {
        try {
            await using FileStream fs = new(openedFile, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true);
            state.OverallMax = fs.Length;
            await using ReadProgressStream rfs = new(fs, p => state.OverallValue = p);
            using var reader = ReaderFactory.Open(rfs);

            while (reader.MoveToNextEntry()) {
                var entry = reader.Entry;

                if (entry.IsDirectory) {
                    Directory.CreateDirectory(Path.Combine(destinationPath, entry.Key!));

                    continue;
                }

                state.CurrentPath = entry.Key;
                state.CurrentMax = entry.Size;
                await using var es = reader.OpenEntryStream();

                await using FileStream efs = new(Path.Combine(destinationPath, entry.Key!), FileMode.CreateNew, FileAccess.Write, FileShare.None,
                    4096, true);

                await using WriteProgressStream wefs = new(efs, p => state.CurrentValue = p);

                await es.CopyToAsync(wefs);

                state.CurrentValue = entry.Size;
            }
        } finally {
            state.OverallValue = state.OverallMax;
            state.IsCompleted = true;
        }
    }

    [RelayCommand]
    private async Task FileSelect() {
        FileOpenPicker fop = new(App.Current.Window.AppWindow.Id);

        fop.FileTypeFilter.Add(".zip");

        var result = await fop.PickSingleFileAsync();

        if (result is not null && File.Exists(result.Path)) {
            openedFile = result.Path;
        }
    }

    [RelayCommand]
    private async Task DecompressFile() {
        if (openedFile is null || !File.Exists(openedFile)) {
            return;
        }

        FolderPicker fp = new(App.Current.Window.AppWindow.Id);
        var result = await fp.PickSingleFolderAsync();

        if (result is not null && Directory.Exists(result.Path)) {
            ProgressWindowViewModel pwvm = new();
            ProgressWindow pw = new(pwvm);

            pw.SetIsMinimizable(false);
            pw.SetIsMaximizable(false);
            pw.SetIsResizable(false);
            pw.CenterOnScreen(800, 320);
            pw.Show();

            await Task.Run(async () => {
                ProgressState state = new();
                var updaterTask = RunUpdaterTask(pw, pwvm, state);
                var decompressorTask = RunDecompressorTask(openedFile, result.Path, state);

                await Task.WhenAll(updaterTask, decompressorTask);
            });

            pw.Close();
        }
    }
}
