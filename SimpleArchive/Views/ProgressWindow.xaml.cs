using SimpleArchive.ViewModels;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace SimpleArchive.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ProgressWindow {
    private readonly ProgressWindowViewModel vm;

    private ProgressWindow(ProgressWindowViewModel vm) {
        this.vm = vm;
        InitializeComponent();
    }

    internal static ProgressWindow Create(ProgressWindowViewModel pwvm) {
        ProgressWindow pw = new(pwvm);

        pw.SetIsMinimizable(false);
        pw.SetIsMaximizable(false);
        pw.SetIsResizable(false);
        pw.CenterOnScreen(800, 320);

        return pw;
    }
}
