using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleArchive.ViewModels;
using System.Runtime.InteropServices;
using WinUIEx;

// ReSharper disable InconsistentNaming
namespace SimpleArchive.Views;

public sealed partial class ProgressWindow {
#pragma warning disable SA1310
    private const int GWLP_HWNDPARENT = -8;
    private const uint WM_CLOSE = 0x0010;
    private const uint SC_CLOSE = 0xF060;
    private const uint MF_DISABLED = 0x00000002;
    private const uint MF_GRAYED = 0x00000001;
#pragma warning restore SA1310
    private readonly ProgressWindowViewModel vm;

    private ProgressWindow(ProgressWindowViewModel vm) {
        this.vm = vm;
        InitializeComponent();

        if (App.Current.MWindow is not null) {
            var presenter = OverlappedPresenter.CreateForDialog();
            presenter.IsModal = true;

            SetWindowLongPtrW(this.GetWindowHandle(), GWLP_HWNDPARENT, App.Current.MWindow.GetWindowHandle());
            AppWindow.SetPresenter(presenter);
        }

        AppWindow.Closing += AppWindow_Closing;
        Closed += ProgressWindow_Closed;
    }

    public bool CloseGracefully() => PostMessageW(this.GetWindowHandle(), WM_CLOSE, 0, 0);

    internal static ProgressWindow Create(ProgressWindowViewModel pwvm) {
        ProgressWindow pw = new(pwvm);

        pw.SetIsMinimizable(false);
        pw.SetIsMaximizable(false);
        pw.SetIsResizable(false);
        pw.CenterOnScreen(800, 320);

        return pw;
    }

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern nint SetWindowLongPtrW(IntPtr hWnd, int index, nint newLong);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PostMessageW(IntPtr hWnd, uint msg, nint wparam, nint lparam);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool revert);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern int EnableMenuItem(IntPtr hMenu, uint idEnableItem, uint enable);

    private async void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args) {
        args.Cancel = true;

        vm.Suspend();

        ContentDialog cd = new() {
            XamlRoot = Content.XamlRoot,
            Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"],
            Title = "Cancel?",
            Content = "Are you sure you want to cancel the process?",
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "No",
            DefaultButton = ContentDialogButton.Secondary
        };

        var value = await cd.ShowAsync() == ContentDialogResult.Primary;

        vm.Resume();

        if (value) {
            vm.Cancel();

            var menu = GetSystemMenu(this.GetWindowHandle(), false);

            _ = EnableMenuItem(menu, SC_CLOSE, MF_DISABLED | MF_GRAYED);
        }
    }

    private void ProgressWindow_Closed(object sender, WindowEventArgs args) => App.Current.MWindow?.Activate();
}
