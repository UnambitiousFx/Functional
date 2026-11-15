using System.Text;

namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     A single validation error, optionally tied to a field and carrying a stable code for clients.
/// </summary>
/// <param name="Field">The name of the invalid field, or <see langword="null" /> for an error not tied to a field.</param>
/// <param name="Message">The human-readable error message.</param>
/// <param name="Code">An optional stable code (for example <c>"Required"</c>) clients can use for i18n or UI mapping.</param>
public sealed record ValidationError(string? Field, string Message, string? Code = null)
{
    /// <summary>
    ///     Gets <see cref="Field" /> as a JSON Pointer (RFC 6901) in URI fragment form, such as <c>#/payload/movements/3/amount</c>,
    ///     or <see langword="null" /> when the error is not tied to a field.
    /// </summary>
    /// <remarks>
    ///     A field already written as a pointer (starting with <c>#</c> or <c>/</c>) is returned unchanged. Otherwise the
    ///     field is read as a dotted and indexed path (<c>payload.movements[3].amount</c>, <c>items["a.b"]</c>,
    ///     <c>$.amount</c>): each segment becomes a pointer token, with <c>~</c> and <c>/</c> escaped as <c>~0</c> and
    ///     <c>~1</c>. Segment names keep their casing.
    /// </remarks>
    public string? Pointer => ToJsonPointer(Field);

    private static string? ToJsonPointer(string? field)
    {
        if (string.IsNullOrEmpty(field)) {
            return null;
        }

        if (field[0] is '#' or '/') {
            return field;
        }

        var pointer = new StringBuilder("#", field.Length + 8);
        var segment = new StringBuilder();
        var first   = true;
        var i       = 0;
        while (i < field.Length) {
            var c = field[i];
            if (c == '.') {
                AppendSegment(pointer, segment, ref first);
                i++;
            }
            else if (c == '[') {
                AppendSegment(pointer, segment, ref first);
                var end = field.IndexOf(']', i + 1);
                if (end < 0) {
                    // Unclosed bracket: keep the rest as a literal segment.
                    segment.Append(field, i, field.Length - i);
                    break;
                }

                var index = field.AsSpan(i + 1, end - i - 1);
                if (index.Length >= 2 &&
                    index[0] is '\'' or '"' &&
                    index[^1] == index[0]) {
                    index = index[1..^1];
                }

                segment.Append(index);
                AppendSegment(pointer, segment, ref first, keepEmpty: true);
                i = end + 1;
            }
            else {
                segment.Append(c);
                i++;
            }
        }

        AppendSegment(pointer, segment, ref first);
        return pointer.ToString();
    }

    private static void AppendSegment(StringBuilder pointer,
                                      StringBuilder segment,
                                      ref bool      first,
                                      bool          keepEmpty = false)
    {
        if (segment.Length == 0 &&
            !keepEmpty) {
            return;
        }

        // "$" is the JSONPath root (System.Text.Json model errors use "$.amount"), not a member.
        var isRoot = first && segment is { Length: 1 } && segment[0] == '$';
        first = false;
        if (!isRoot) {
            pointer.Append('/');
            for (var j = 0; j < segment.Length; j++) {
                switch (segment[j]) {
                    case '~':
                        pointer.Append("~0");
                        break;
                    case '/':
                        pointer.Append("~1");
                        break;
                    default:
                        pointer.Append(segment[j]);
                        break;
                }
            }
        }

        segment.Clear();
    }
}
