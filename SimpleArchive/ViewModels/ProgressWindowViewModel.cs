using CommunityToolkit.Mvvm.ComponentModel;

namespace SimpleArchive.ViewModels;

internal sealed partial class ProgressWindowViewModel : ViewModel {
    [ObservableProperty]
    public partial double CurrentValue { get; set; }

    [ObservableProperty]
    public partial double MaxCurrent { get; set; }

    [ObservableProperty]
    public partial double ArchiveValue { get; set; }

    [ObservableProperty]
    public partial double MaxArchive { get; set; }

    [ObservableProperty]
    public partial string? CurrentPath { get; set; }
}
