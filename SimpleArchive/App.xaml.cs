using Microsoft.UI.Xaml;
using SimpleArchive.Models;
using SimpleArchive.Views;
using WinUIEx;

namespace SimpleArchive;

public sealed partial class App {
    public App() => InitializeComponent();

    public static new App Current => (App)Application.Current;

    public Window? MWindow { get; private set; }

    protected override async void OnLaunched(LaunchActivatedEventArgs args) {
        var runArgs = Environment.GetCommandLineArgs();

        switch (runArgs.Length) {
            case 1:
                MWindow = new MainWindow();
                MWindow.CenterOnScreen(800, 600);
                MWindow.Activate();

                break;

            case 4:
                var operation = Operation.None;

                if (runArgs[1].Equals("extractsmart", StringComparison.OrdinalIgnoreCase)) {
                    operation = Operation.ExtractSmart;
                }

                var fromPath = runArgs[2];
                var destinationPath = runArgs[3];

                Func<Task> task = operation switch {
                    Operation.ExtractSmart => async Task () => {
                        if (!File.Exists(fromPath) || !Directory.Exists(destinationPath)) {
                            return;
                        }

                        await using var decompressor = DecompressorFactory.Create(fromPath, destinationPath, true);

                        decompressor.ProgressWindow.AppWindow.Show(true);
                        await Task.Run(decompressor.DecompressAsync);
                        decompressor.ProgressWindow.Close();
                    }
                    ,
                    _ => () => Task.CompletedTask
                };

                await task();

                break;
        }
    }
}
