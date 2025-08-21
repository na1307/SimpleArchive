namespace SimpleArchive.ShellExtension;

[Flags]
public enum Expcmdstate : uint {
    Enabled = 0,
    Disabled = 0x1,
    Hidden = 0x2,
    Checkbox = 0x4,
    Checked = 0x8,
    Radiocheck = 0x10
}
