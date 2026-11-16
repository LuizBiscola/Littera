using Notas.Editor.Languages;
using ScintillaNET;

namespace Notas.Editor;

/// <summary>
/// Configuração do controle Scintilla (versão da Fase 0: base visual + SQL).
/// Na Fase 1 isto vira EditorHost + LanguageRegistry.
/// </summary>
internal static class EditorSetup
{
    private static readonly Color Text = ColorTranslator.FromHtml("#1F2328");
    private static readonly Color Background = Color.White;
    private static readonly Color Gutter = ColorTranslator.FromHtml("#B4B8BF");
    private static readonly Color Comment = ColorTranslator.FromHtml("#8B929B");
    private static readonly Color Keyword = ColorTranslator.FromHtml("#1F4FD1");
    private static readonly Color StringColor = ColorTranslator.FromHtml("#B4371F");
    private static readonly Color Function = ColorTranslator.FromHtml("#8A3FB8");
    private static readonly Color Number = ColorTranslator.FromHtml("#0B7A6B");
    private static readonly Color DataType = ColorTranslator.FromHtml("#0E7490");
    private static readonly Color Variable = ColorTranslator.FromHtml("#9A6700");
    private static readonly Color CurrentLine = ColorTranslator.FromHtml("#EAF1FE");

    public static void ConfigureView(Scintilla editor, Technology technology)
    {
        editor.BorderStyle = ScintillaNET.BorderStyle.None;
        editor.Technology = technology;

        editor.StyleResetDefault();
        Style defaultStyle = editor.Styles[Style.Default];
        defaultStyle.Font = PickMonospaceFont();
        defaultStyle.SizeF = 10f;
        defaultStyle.ForeColor = Text;
        defaultStyle.BackColor = Background;
        editor.StyleClearAll();

        editor.ExtraAscent = 2;
        editor.ExtraDescent = 3;

        editor.Styles[Style.LineNumber].ForeColor = Gutter;
        editor.Styles[Style.LineNumber].BackColor = Background;
        editor.Margins[0].Type = MarginType.Number;
        editor.Margins[0].Width = editor.TextWidth(Style.LineNumber, "99999") + 8;
        editor.Margins[1].Width = 12;

        editor.CaretLineBackColor = CurrentLine; // cor opaca = linha atual visível
        editor.CaretForeColor = Text;
        editor.CaretWidth = 2;

        editor.TabWidth = 4;
        editor.UseTabs = false;

        // Múltiplos cursores e seleção em coluna (Alt + arrastar) são nativos do Scintilla.
        editor.MultipleSelection = true;
        editor.AdditionalSelectionTyping = true;
        editor.MouseSelectionRectangularSwitch = true;
        editor.MultiPaste = MultiPaste.Each;
        editor.VirtualSpaceOptions = VirtualSpace.RectangularSelection;
    }

    /// <summary>O lexer pertence ao documento: precisa ser reaplicado sempre que o documento troca.</summary>
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

    private static string PickMonospaceFont()
    {
        foreach (string name in new[] { "Cascadia Code", "Consolas" })
        {
            using var font = new Font(name, 10f);
            if (string.Equals(font.Name, name, StringComparison.OrdinalIgnoreCase))
                return name;
        }

        return "Courier New";
    }
}
