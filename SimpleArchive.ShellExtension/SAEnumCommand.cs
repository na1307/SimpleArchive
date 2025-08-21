namespace SimpleArchive.ShellExtension;

[GeneratedComClass]
[Guid("18320B09-DE68-4D52-81CF-C92A532D1289")]
public sealed partial class SAEnumCommand : IEnumExplorerCommand {
    private static readonly StrategyBasedComWrappers Sbcw = new();
    private static readonly Guid IID_IExplorerCommand = Guid.Parse("a08ce4d0-fa25-44ab-b57c-c7b1c323e0b9");
    private readonly IExplorerCommand[] commands = [new SmartExtractCommand()];
    private uint read;

    public unsafe int Next(uint celt, void** command, uint* celtFetched) {
        if (celt != 1 || commands.Length < read + celt) {
            *command = null;

            if (celtFetched is not null) {
                *celtFetched = 0;
            }

            return 1;
        }

        var p = Sbcw.GetOrCreateComInterfaceForObject(commands[read], CreateComInterfaceFlags.None);
        var hr = Marshal.QueryInterface(p, in IID_IExplorerCommand, out var ppv);

        Marshal.Release(p);

        if (hr == 0) {
            *command = ppv.ToPointer();

            if (celtFetched is not null) {
                *celtFetched = 1;
            }

            read += celt;
        }

        return hr;
    }

    public void Skip(uint celt) => throw new NotImplementedException();

    public void Reset() => throw new NotImplementedException();

    public IEnumExplorerCommand Clone() => throw new NotImplementedException();
}
