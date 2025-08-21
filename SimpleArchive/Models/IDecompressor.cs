using SimpleArchive.Views;

namespace SimpleArchive.Models;

internal interface IDecompressor : IDisposable, IAsyncDisposable {
    ProgressWindow ProgressWindow { get; }

    Task DecompressAsync();
}
