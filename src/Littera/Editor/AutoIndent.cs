using ScintillaNET;

namespace Littera.Editor;

/// <summary>Enter mantém a indentação da linha de cima, em vez de voltar para a margem esquerda.</summary>
internal static class AutoIndent
{
    public static void Attach(Scintilla editor) => editor.CharAdded += (_, e) => OnCharAdded(editor, e.Char);

    private static void OnCharAdded(Scintilla editor, int added)
    {
        bool newLine = added == '\n' || (added == '\r' && editor.EolMode == Eol.Cr);
        if (!newLine || editor.Selections.Count > 1)
            return;

        int line = editor.CurrentLine;
        int indent = line > 0 ? editor.Lines[line - 1].Indentation : 0;
        if (indent == 0)
            return;
        editor.Lines[line].Indentation = indent; // usa tabs ou espaços conforme a configuração
        if (string.IsNullOrWhiteSpace(editor.Lines[line - 1].Text))
            editor.Lines[line - 1].Indentation = 0; // linha que ficou só com recuo não guarda espaços sobrando
        editor.GotoPosition(editor.Lines[line].IndentPosition);
    }
}
