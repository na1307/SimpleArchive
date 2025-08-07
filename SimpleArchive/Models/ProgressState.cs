namespace SimpleArchive.Models;

internal sealed class ProgressState {
    public long OverallMax { get; set; }

    public long OverallValue { get; set; }

    public string? CurrentPath { get; set; }

    public long CurrentMax { get; set; }

    public long CurrentValue { get; set; }

    public bool IsCompleted { get; set; }
}
