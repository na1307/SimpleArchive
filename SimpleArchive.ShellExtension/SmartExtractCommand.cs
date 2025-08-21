using System.Diagnostics;

namespace SimpleArchive.ShellExtension;

[GeneratedComClass]
[Guid("3BAA77C5-0171-4AF7-9B0E-08287E2C5AF7")]
public unsafe partial class SmartExtractCommand : IExplorerCommand {
    public string GetTitle(IShellItemArray? itemArray) => "Smart Extract";

    public string GetIcon(IShellItemArray? itemArray) => throw new NotImplementedException();

    public string GetToolTip(IShellItemArray? itemArray) => throw new NotImplementedException();

    public Guid GetCanonicalName() => throw new NotImplementedException();

    public Expcmdstate GetState(IShellItemArray? itemArray, bool okToBeSlow) {
        if (itemArray is null || itemArray.GetCount() > 1) {
            return Expcmdstate.Hidden;
        }

        var item = itemArray.GetItemAt(0);

        if (item is null) {
            return Expcmdstate.Hidden;
        }

        var archivePath = item.GetDisplayName(Sigdn.Desktopabsoluteediting);

        if (CompressionFormatDetector.Detect(archivePath) == SupportedFormat.None) {
            return Expcmdstate.Hidden;
        }

        return Expcmdstate.Enabled;
    }

    public void Invoke(IShellItemArray? itemArray, void* bc) {
        var se = GetModuleHandleW("SimpleArchive.ShellExtension.dll");

        if (se is null) {
            _ = MessageBoxW(null, $"GetModuleHandle failed: {Marshal.GetLastPInvokeErrorMessage()}", null, 16);

            return;
        }

        const int bufferSize = 260;
        var buffer = stackalloc char[bufferSize];
        var u = GetModuleFileNameW(se, buffer, bufferSize);

        if (u == 0) {
            _ = MessageBoxW(null, $"GetModuleFileName failed: {Marshal.GetLastPInvokeErrorMessage()}", null, 16);

            return;
        }

        if (itemArray is null) {
            _ = MessageBoxW(null, "Item array is null.", null, 16);

            return;
        }

        if (itemArray.GetCount() > 1) {
            _ = MessageBoxW(null, "Item array has more than one item", null, 16);

            return;
        }

        var item = itemArray.GetItemAt(0);

        if (item is null) {
            _ = MessageBoxW(null, "Item is null.", null, 16);

            return;
        }

        var archivePath = item.GetDisplayName(Sigdn.Desktopabsoluteediting);

        if (!File.Exists(archivePath)) {
            _ = MessageBoxW(null, $"File not found: {archivePath}", null, 16);

            return;
        }

        var exePath = Path.Combine(Path.GetDirectoryName(new(buffer))!, "SimpleArchive.exe");

        if (!File.Exists(exePath)) {
            _ = MessageBoxW(null, $"SimpleArchive.exe not found: {exePath}", null, 16);

            return;
        }

        var p = Process.Start(new ProcessStartInfo(exePath, ["extractsmart", archivePath, Path.GetDirectoryName(exePath)!]));

        if (p is null) {
            _ = MessageBoxW(null, "SimpleArchive launch failed.", null, 16);
        }
    }

    public Expcmdflags GetFlags() => Expcmdflags.Default;

    public IEnumExplorerCommand EnumSubCommands() => throw new NotImplementedException();

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern int MessageBoxW(
        void* hwnd,
        [MarshalAs(UnmanagedType.LPWStr)] string? text,
        [MarshalAs(UnmanagedType.LPWStr)] string? caption,
        uint type);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern void* GetModuleHandleW([MarshalAs(UnmanagedType.LPWStr)] string moduleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern uint GetModuleFileNameW(void* module, char* filename, uint size);
}
