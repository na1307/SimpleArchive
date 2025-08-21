namespace SimpleArchive.ShellExtension;

[GeneratedComInterface]
[Guid("a88826f8-186f-4987-aade-ea0cef8fbfe8")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public unsafe partial interface IEnumExplorerCommand {
    [PreserveSig]
    int Next(uint celt, void** command, uint* celtFetched);

    void Skip(uint celt);

    void Reset();

    IEnumExplorerCommand? Clone();
}
