using CommunityToolkit.Mvvm.Input;
using Microsoft.Windows.Storage.Pickers;
using SimpleArchive.Models;
using SimpleArchive.Views;
using WinUIEx;

namespace SimpleArchive.ViewModels;

internal sealed partial class MainWindowViewModel : ViewModel {
    private string? openedArchive;

    [RelayCommand]
    private async Task FileSelect() {
        FileOpenPicker fop = new(App.Current.Window.AppWindow.Id);

        fop.FileTypeFilter.Add(".zip");

        var result = await fop.PickSingleFileAsync();

        if (result is not null && File.Exists(result.Path)) {
            openedArchive = result.Path;
        }
    }

    [RelayCommand]
    private async Task DecompressFile() {
        if (openedArchive is null || !File.Exists(openedArchive)) {
            return;
        }

        FolderPicker fp = new(App.Current.Window.AppWindow.Id);
        var result = await fp.PickSingleFolderAsync();

        if (result is not null && Directory.Exists(result.Path)) {
            ProgressWindowViewModel pwvm = new();
            var pw = ProgressWindow.Create(pwvm);

            pw.Show();
            await Task.Run(() => Decompressor.Decompress(openedArchive, pw, pwvm, result.Path));
            pw.Close();
        }
    }
}
