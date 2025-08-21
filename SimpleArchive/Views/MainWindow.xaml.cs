using SimpleArchive.ViewModels;

namespace SimpleArchive.Views;

public sealed partial class MainWindow {
    private readonly MainWindowViewModel vm = new();

    public MainWindow() => InitializeComponent();
}
