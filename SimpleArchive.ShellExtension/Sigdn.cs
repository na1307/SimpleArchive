namespace SimpleArchive.ShellExtension;

public enum Sigdn {
    Normaldisplay = 0,
    Parentrelativeparsing = unchecked((int)0x80018001),
    Desktopabsoluteparsing = unchecked((int)0x80028000),
    Parentrelativeediting = unchecked((int)0x80031001),
    Desktopabsoluteediting = unchecked((int)0x8004c000),
    Filesyspath = unchecked((int)0x80058000),
    Url = unchecked((int)0x80068000),
    Parentrelativeforaddressbar = unchecked((int)0x8007c001),
    Parentrelative = unchecked((int)0x80080001),
    Parentrelativeforui = unchecked((int)0x80094001)
}
