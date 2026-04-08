using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace UnambitiousFx.Functional;

[ExcludeFromCodeCoverage]
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
