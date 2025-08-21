namespace SimpleArchive.Models;

internal static class DecompressorFactory {
    public static IDecompressor Create(string archivePath, string destinationPath, bool extractSmart)
        => CompressionFormatDetector.Detect(archivePath) switch {
            SupportedFormat.Zip => new ZipDecompressor(archivePath, destinationPath, extractSmart),
            _ => throw new NotSupportedException()
        };
}
