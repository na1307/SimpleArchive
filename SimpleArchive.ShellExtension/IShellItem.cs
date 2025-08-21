namespace SimpleArchive.ShellExtension;

[GeneratedComInterface]
[Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public unsafe partial interface IShellItem {
    void* BindToHandler(void* bc, in Guid bhid, in Guid iid);

    IShellItem? GetParent();

    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetDisplayName(Sigdn sigdnName);

    [PreserveSig]
    int GetAttributes(uint sfgaoMask, out uint sfgaoAttribs);

    [PreserveSig]
    int Compare(IShellItem si, uint hint, out int order);
}
