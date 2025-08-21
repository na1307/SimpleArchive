namespace SimpleArchive.ShellExtension;

[GeneratedComClass]
[Guid("059F8A2D-271B-415E-9267-18B9E4B164DC")]
public sealed unsafe partial class SACommand : IExplorerCommand {
    public string GetTitle(IShellItemArray? itemArray) => "SimpleArchive";

    public string GetIcon(IShellItemArray? itemArray) => throw new NotImplementedException();

    public string GetToolTip(IShellItemArray? itemArray) => throw new NotImplementedException();

    public Guid GetCanonicalName() => throw new NotImplementedException();

    public Expcmdstate GetState(IShellItemArray? itemArray, bool okToBeSlow) => Expcmdstate.Enabled;

    public void Invoke(IShellItemArray? itemArray, void* bc) => throw new NotImplementedException();

    public Expcmdflags GetFlags() => Expcmdflags.Hassubcommands;

    public IEnumExplorerCommand EnumSubCommands() => new SAEnumCommand();
}
