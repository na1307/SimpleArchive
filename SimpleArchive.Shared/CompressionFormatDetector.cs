namespace SimpleArchive;

public static class CompressionFormatDetector {
    public static SupportedFormat Detect(string archivePath) {
        Span<byte> span = new byte[4];

        using (var readfs = File.OpenRead(archivePath)) {
            readfs.ReadExactly(span);
        }

        return span switch {
            [0x50, 0x4B, 0x03, 0x04, ..] /* Zip */ => SupportedFormat.Zip,
            _ => SupportedFormat.None
        };
    }
}
