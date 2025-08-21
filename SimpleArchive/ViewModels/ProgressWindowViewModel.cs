using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleArchive.Views;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SimpleArchive.ViewModels;

internal sealed partial class ProgressWindowViewModel(CancellationTokenSource cts, ManualResetEventSlim mres) : ViewModel {
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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Canceling))]
    public partial bool CancelEnabled { get; private set; } = true;

    public string Canceling => CancelEnabled ? "Cancel" : "Canceling...";

    public void Suspend() => mres.Reset();

    public void Resume() => mres.Set();

    public void Cancel() {
        if (!cts.IsCancellationRequested) {
            CancelEnabled = false;
            cts.Cancel();
        }
    }

    [RelayCommand]
    private void CancelButton(ProgressWindow pw) {
        if (!cts.IsCancellationRequested && !pw.CloseGracefully()) {
            throw new Win32Exception(Marshal.GetLastPInvokeError());
        }
    }
}
