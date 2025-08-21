namespace SimpleArchive.ShellExtension;

[GeneratedComInterface]
[Guid("b63ea76d-1f85-456f-a19c-48159efa858b")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public unsafe partial interface IShellItemArray {
    void* BindToHandler(void* bc, in Guid bhid, in Guid iid);

    void* GetPropertyStore(int flags, in Guid iid);

    void* GetPropertyDescriptionList(void* keyType, in Guid iid);

    [PreserveSig]
    int GetAttributes(int attribFlags, uint sfgaoMask, out uint sfgaoAttribs);

    uint GetCount();

    IShellItem? GetItemAt(uint index);

    void* EnumItems();
}
