using ScintillaNET;

namespace Notas.Editor.Highlights;

/// <summary>
/// Cursor encostado num "(" ou ")" (também [ ] e { }): destaca o par; sem par, marca em vermelho.
/// O Scintilla só casa símbolos do mesmo estilo, então parênteses dentro de strings e comentários não contam.
/// </summary>
internal static class BraceMatcher
{
    public static void Attach(Scintilla editor) => editor.UpdateUI += (_, e) =>
    {
        if ((e.Change & (UpdateChange.Selection | UpdateChange.Content)) != 0)
            Update(editor);
    };

    private static void Update(Scintilla editor)
    {
        int caret = editor.CurrentPosition;
        int brace = caret > 0 && IsBrace(editor.GetCharAt(caret - 1)) ? caret - 1 // "COUNT(|" e "x)|": o que acabou de ser digitado
            : caret < editor.TextLength && IsBrace(editor.GetCharAt(caret)) ? caret : -1;
        if (brace < 0 || editor.SelectionStart != editor.SelectionEnd)
        {
            editor.BraceHighlight(-1, -1);
            return;
        }

        int match = editor.BraceMatch(brace);
        if (match < 0)
            editor.BraceBadLight(brace);
        else
            editor.BraceHighlight(brace, match);
    }

    private static bool IsBrace(char c) => c is '(' or ')' or '[' or ']' or '{' or '}';
}
