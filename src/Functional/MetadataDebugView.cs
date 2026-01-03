using System.Diagnostics;

namespace UnambitiousFx.Functional;

internal sealed class MetadataDebugView
{
    private readonly Metadata _metadata;

    public MetadataDebugView(Metadata metadata)
    {
        _metadata = metadata;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public KeyValuePair<string, object?>[] Items => _metadata.ToArray();
}
