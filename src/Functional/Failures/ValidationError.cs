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
    ///     field is read as a dotted and indexed path (<c>payload.movements[3].amount</c>, <c>items['a.b']</c>,
    ///     <c>$.amount</c>): each segment becomes a pointer token, with <c>~</c> and <c>/</c> escaped as <c>~0</c> and
    ///     <c>~1</c>, then any character not allowed in a URI fragment percent-encoded as UTF-8 (<c>first name</c> →
    ///     <c>first%20name</c>, <c>%</c> → <c>%25</c>). A quoted key (<c>['…']</c> or <c>["…"]</c>) is taken
    ///     literally, <c>\</c> escaping the next character. Segment names keep their casing.
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
        var token   = new StringBuilder();
        var i       = 0;

        // An unquoted leading "$" is the JSONPath root (System.Text.Json model errors use "$.amount"), not a member.
        if (field[0] == '$' &&
            (field.Length == 1 || field[1] is '.' or '[')) {
            i = 1;
        }

        while (i < field.Length) {
            var c = field[i];
            if (c == '.') {
                AppendToken(pointer, token, false);
                i++;
            }
            else if (c == '[') {
                AppendToken(pointer, token, false);
                var end = ReadBracket(field, i, token);
                if (end < 0) {
                    // Unclosed bracket: keep the rest as a literal token.
                    token.Append(field, i, field.Length - i);
                    break;
                }

                AppendToken(pointer, token, true);
                i = end + 1;
            }
            else {
                token.Append(c);
                i++;
            }
        }

        AppendToken(pointer, token, false);
        return pointer.ToString();
    }

    /// <summary>
    ///     Reads the bracket opened at <paramref name="start" /> into <paramref name="token" />, unquoting a quoted key.
    /// </summary>
    /// <returns>The index of the closing <c>]</c>, or -1 when the bracket is not closed.</returns>
    private static int ReadBracket(string        field,
                                   int           start,
                                   StringBuilder token)
    {
        var i = start + 1;
        if (i < field.Length &&
            field[i] is '\'' or '"') {
            var quote = field[i];
            for (i++; i < field.Length; i++) {
                var c = field[i];
                if (c == '\\' &&
                    i + 1 < field.Length) {
                    token.Append(field[++i]);
                    continue;
                }

                if (c == quote) {
                    if (i + 1 < field.Length &&
                        field[i + 1] == ']') {
                        return i + 1;
                    }

                    break;
                }

                token.Append(c);
            }

            // Not a well-formed quoted key: read it as a plain bracket.
            token.Clear();
            i = start + 1;
        }

        var end = field.IndexOf(']', i);
        if (end < 0) {
            return -1;
        }

        token.Append(field, i, end - i);
        return end;
    }

    private static void AppendToken(StringBuilder pointer,
                                    StringBuilder token,
                                    bool          keepEmpty)
    {
        if (token.Length == 0 &&
            !keepEmpty) {
            return;
        }

        pointer.Append('/');
        Span<byte> utf8 = stackalloc byte[4];
        foreach (var rune in token.ToString().EnumerateRunes()) {
            if (rune.Value == '~') {
                pointer.Append("~0");
            }
            else if (rune.Value == '/') {
                pointer.Append("~1");
            }
            else if (IsFragmentChar(rune)) {
                pointer.Append((char)rune.Value);
            }
            else {
                var length = rune.EncodeToUtf8(utf8);
                foreach (var b in utf8[..length]) {
                    pointer.Append('%').Append(b.ToString("X2"));
                }
            }
        }

        token.Clear();
    }

    // RFC 3986 fragment characters, except "/" which JSON Pointer escapes itself.
    private static bool IsFragmentChar(Rune rune)
    {
        return rune.IsAscii &&
               (char.IsAsciiLetterOrDigit((char)rune.Value) ||
                "-._~!$&'()*+,;=:@?".Contains((char)rune.Value));
    }
}
