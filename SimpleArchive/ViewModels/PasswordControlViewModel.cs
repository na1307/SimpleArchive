using CommunityToolkit.Mvvm.ComponentModel;

namespace SimpleArchive.ViewModels;

internal sealed partial class PasswordControlViewModel : ViewModel {
    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;
}
