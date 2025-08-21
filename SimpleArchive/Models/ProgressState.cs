namespace SimpleArchive.Models;

internal sealed class ProgressState {
    private readonly Lock locker = new();

    public long OverallMax {
        get {
            lock (locker) {
                return field;
            }
        }

        set {
            lock (locker) {
                field = value;
            }
        }
    }

    public long OverallValue {
        get {
            lock (locker) {
                return field;
            }
        }

        set {
            lock (locker) {
                field = value;
            }
        }
    }

    public string? CurrentPath {
        get {
            lock (locker) {
                return field;
            }
        }

        set {
            lock (locker) {
                field = value;
            }
        }
    }

    public long CurrentMax {
        get {
            lock (locker) {
                return field;
            }
        }

        set {
            lock (locker) {
                field = value;
            }
        }
    }

    public long CurrentValue {
        get {
            lock (locker) {
                return field;
            }
        }

        set {
            lock (locker) {
                field = value;
            }
        }
    }
}
