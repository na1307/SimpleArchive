using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Windows.Storage.Pickers;
using SimpleArchive.Models;

namespace SimpleArchive.ViewModels;

internal sealed partial class MainWindowViewModel : ViewModel {
    [ObservableProperty]
    private partial string? OpenedArchive { get; set; }

    [RelayCommand]
    private async Task FileSelect() {
        FileOpenPicker fop = new(App.Current.MWindow!.AppWindow.Id);

        fop.FileTypeFilter.Add(".zip");

        var result = await fop.PickSingleFileAsync();

        if (result is not null && File.Exists(result.Path)) {
            OpenedArchive = result.Path;
        }
    }

    [RelayCommand]
    private async Task DecompressFile() {
        if (OpenedArchive is null || !File.Exists(OpenedArchive)) {
            return;
        }

        FolderPicker fp = new(App.Current.MWindow!.AppWindow.Id);
        var result = await fp.PickSingleFolderAsync();

        if (result is not null && Directory.Exists(result.Path)) {
            await using var decompressor = DecompressorFactory.Create(OpenedArchive, result.Path, false);

            decompressor.ProgressWindow.AppWindow.Show(true);
            await Task.Run(decompressor.DecompressAsync);
            decompressor.ProgressWindow.Close();
        }
    }
}
