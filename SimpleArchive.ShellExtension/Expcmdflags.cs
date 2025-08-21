namespace SimpleArchive.ShellExtension;

[Flags]
public enum Expcmdflags : uint {
    Default = 0,
    Hassubcommands = 0x1,
    Hassplitbutton = 0x2,
    Hidelabel = 0x4,
    Isseparator = 0x8,
    Hasluashield = 0x10,
    Separatorbefore = 0x20,
    Separatorafter = 0x40,
    Isdropdown = 0x80,
    Toggleable = 0x100,
    Automenuicons = 0x200
}
