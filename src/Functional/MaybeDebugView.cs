namespace UnambitiousFx.Functional;

internal sealed class MaybeDebugView<TValue>
    where TValue : notnull
{
    private readonly Maybe<TValue> _maybe;

    public MaybeDebugView(Maybe<TValue> maybe)
    {
        _maybe = maybe;
    }

    public bool IsSome => _maybe.IsSome;
    public bool IsNone => _maybe.IsNone;
    public bool HasValue => _maybe.IsSome;
    public TValue? Value => _maybe.Some(out var value) ? value : default;
}
