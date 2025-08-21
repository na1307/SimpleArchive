namespace SimpleArchive.ShellExtension;

[GeneratedComInterface]
[Guid("a08ce4d0-fa25-44ab-b57c-c7b1c323e0b9")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public unsafe partial interface IExplorerCommand {
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string? GetTitle(IShellItemArray? itemArray);

    [return: MarshalAs(UnmanagedType.LPWStr)]
    string? GetIcon(IShellItemArray? itemArray);

    [return: MarshalAs(UnmanagedType.LPWStr)]
    string? GetToolTip(IShellItemArray? itemArray);

    Guid GetCanonicalName();

    Expcmdstate GetState(IShellItemArray? itemArray, [MarshalAs(UnmanagedType.Bool)] bool okToBeSlow);

    void Invoke(IShellItemArray? itemArray, void* bc);

    Expcmdflags GetFlags();

    IEnumExplorerCommand EnumSubCommands();
}
