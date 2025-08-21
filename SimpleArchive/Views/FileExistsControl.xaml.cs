using SimpleArchive.ViewModels;

namespace SimpleArchive.Views;

public sealed partial class FileExistsControl {
    private readonly FileExistsControlViewModel vm;

    internal FileExistsControl(FileExistsControlViewModel vm) {
        this.vm = vm;
        InitializeComponent();
    }
}
