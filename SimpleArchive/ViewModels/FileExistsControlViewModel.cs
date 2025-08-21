using CommunityToolkit.Mvvm.ComponentModel;

namespace SimpleArchive.ViewModels;

internal sealed partial class FileExistsControlViewModel(string dest) : ViewModel {
    public string Text => $"Do you want to overwrite the file \"{dest}\"?";

    [ObservableProperty]
    public partial bool Remember { get; set; }
}
