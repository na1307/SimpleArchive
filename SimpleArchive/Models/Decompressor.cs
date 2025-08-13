using SharpCompress.Readers;
using SimpleArchive.ViewModels;
using SimpleArchive.Views;

namespace SimpleArchive.Models;

internal static class Decompressor {
    public static Task Decompress(string archivePath, ProgressWindow pw, ProgressWindowViewModel pwvm, string destinationPath) {
        ProgressState state = new();
        var updaterTask = RunUpdaterTask(pw, pwvm, state);
        var decompressorTask = RunDecompressorTask(archivePath, destinationPath, state);

        return Task.WhenAll(updaterTask, decompressorTask);
    }

    private static async Task RunUpdaterTask(ProgressWindow pw, ProgressWindowViewModel pwvm, ProgressState state) {
        while (!state.IsCompleted) {
            await Task.Delay(100);

            pw.DispatcherQueue.TryEnqueue(() => {
                pwvm.MaxOverall = state.OverallMax;
                pwvm.OverallValue = state.OverallValue;
                pwvm.CurrentPath = state.CurrentPath;
                pwvm.MaxCurrent = state.CurrentMax;
                pwvm.CurrentValue = state.CurrentValue;
            });
        }
    }

    private static async Task RunDecompressorTask(string archivePath, string destinationPath, ProgressState state) {
        try {
            await using FileStream fs = new(archivePath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true);
            state.OverallMax = fs.Length;
            await using ReadProgressStream rfs = new(fs, p => state.OverallValue = p);
            using var reader = ReaderFactory.Open(rfs);

            while (reader.MoveToNextEntry()) {
                var entry = reader.Entry;
                var dest = Path.Combine(destinationPath, entry.Key!);

                if (entry.IsDirectory) {
                    Directory.CreateDirectory(dest);

                    continue;
                }

                state.CurrentPath = entry.Key;
                state.CurrentMax = entry.Size;
                state.CurrentValue = 0L;

                await using (FileStream efs = new(dest, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, true)) {
                    await using (WriteProgressStream wefs = new(efs, p => state.CurrentValue = p, true))
                    await using (var es = reader.OpenEntryStream()) {
                        await es.CopyToAsync(wefs);
                    }

                    if (entry.LastModifiedTime is not null) {
                        File.SetLastWriteTime(efs.SafeFileHandle, entry.LastModifiedTime.Value);
                    }
                }

                state.CurrentValue = entry.Size;
            }

            state.OverallValue = fs.Length;
        } finally {
            state.IsCompleted = true;
        }
    }
}
