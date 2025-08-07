using CommunityToolkit.Mvvm.ComponentModel;

namespace SimpleArchive.ViewModels;

internal sealed partial class ProgressWindowViewModel : ViewModel {
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentPercentage))]
    public partial double CurrentValue { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentPercentage))]
    public partial double MaxCurrent { get; set; }

    public string CurrentPercentage => $"{(int)(CurrentValue / MaxCurrent * 100)}%";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OverallPercentage))]
    public partial double OverallValue { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OverallPercentage))]
    public partial double MaxOverall { get; set; }

    public string OverallPercentage => $"{(int)(OverallValue / MaxOverall * 100)}%";

    [ObservableProperty]
    public partial string? CurrentPath { get; set; }
}
