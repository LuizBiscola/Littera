using Notas.Editor.Languages;
using ScintillaNET;

namespace Notas.Editor;

/// <summary>Realce de SQL da Fase 0; dá lugar ao registro de linguagens na Fase 1.</summary>
internal static class EditorSetup
{
    private static readonly Color Comment = ColorTranslator.FromHtml("#8B929B");
    private static readonly Color Keyword = ColorTranslator.FromHtml("#1F4FD1");
    private static readonly Color StringColor = ColorTranslator.FromHtml("#B4371F");
    private static readonly Color Function = ColorTranslator.FromHtml("#8A3FB8");
    private static readonly Color Number = ColorTranslator.FromHtml("#0B7A6B");
    private static readonly Color DataType = ColorTranslator.FromHtml("#0E7490");
    private static readonly Color Variable = ColorTranslator.FromHtml("#9A6700");

    /// <summary>
    /// Usa o lexer "mssql" da Lexilla: realça igual ao "sql" no ANSI e ainda distingue
    /// @variáveis e @@globais do T-SQL (comparação feita na Fase 0).
    /// </summary>
    public static void ApplySql(Scintilla editor)
    {
        editor.LexerName = "mssql";
        editor.SetKeywords(0, SqlKeywords.Keywords);
        editor.SetKeywords(1, SqlKeywords.DataTypes);
        editor.SetKeywords(4, SqlKeywords.Functions);

        SetStyle(editor, 1, Comment, italic: true);   // COMMENT /* */
        SetStyle(editor, 2, Comment, italic: true);   // LINE_COMMENT --
        SetStyle(editor, 3, Number);                  // NUMBER
        SetStyle(editor, 4, StringColor);             // STRING
        SetStyle(editor, 7, Variable);                // @variavel
        SetStyle(editor, 9, Keyword);                 // STATEMENT (palavras-chave)
        SetStyle(editor, 10, DataType);               // DATATYPE
        SetStyle(editor, 12, Variable);               // @@global
        SetStyle(editor, 13, Function);               // FUNCTION
        SetStyle(editor, 15, DataType);               // DEFAULT_PREF_DATATYPE
    }

    private static void SetStyle(Scintilla editor, int style, Color fore, bool italic = false)
    {
        editor.Styles[style].ForeColor = fore;
        editor.Styles[style].Italic = italic;
    }
}
