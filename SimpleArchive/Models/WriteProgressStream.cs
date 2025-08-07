namespace SimpleArchive.Models;

internal sealed partial class WriteProgressStream(Stream baseStream, Action<long> progressCallback) : Stream {
    private long totalBytesWrite;

    public override bool CanRead => baseStream.CanRead;

    public override bool CanSeek => baseStream.CanSeek;

    public override bool CanWrite => baseStream.CanWrite;

    public override long Length => baseStream.Length;

    public override long Position {
        get => baseStream.Position;
        set => baseStream.Position = value;
    }

    public override void Flush() => baseStream.Flush();

    public override int Read(byte[] buffer, int offset, int count) => baseStream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => baseStream.Seek(offset, origin);

    public override void SetLength(long value) => baseStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) {
        baseStream.Write(buffer, offset, count);

        totalBytesWrite += count;

        progressCallback(totalBytesWrite);
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            baseStream.Dispose();
        }

        base.Dispose(disposing);
    }
}
