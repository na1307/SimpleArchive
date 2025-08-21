using SimpleArchive.ViewModels;

namespace SimpleArchive.Views;

public sealed partial class PasswordControl {
    private readonly PasswordControlViewModel vm;

    internal PasswordControl(PasswordControlViewModel vm) {
        this.vm = vm;
        InitializeComponent();
    }
}
