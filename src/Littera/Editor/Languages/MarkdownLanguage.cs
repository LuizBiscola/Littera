using Littera.Theme;
using ScintillaNET;

namespace Littera.Editor.Languages;

internal sealed class MarkdownLanguage : Language
{
    public override string Name => "Markdown";

    public override string? Lexer => "markdown";

    public override string[] Extensions => [".md", ".markdown", ".mdx"];

    protected override void Configure(Scintilla editor)
    {
        for (int header = 6; header <= 11; header++)
        {
            editor.Styles[header].ForeColor = SyntaxColors.Heading;
            editor.Styles[header].Bold = true;
        }

        editor.Styles[2].Bold = editor.Styles[3].Bold = true;          // **forte**
        editor.Styles[4].Italic = editor.Styles[5].Italic = true;      // *ênfase*
        Paint(editor, SyntaxColors.Variable, 13, 14);                  // marcadores de lista
        PaintItalic(editor, SyntaxColors.Comment, 15, 17);             // > citação e ---
        Paint(editor, Palette.TextFaint, 16);                          // ~~riscado~~
        Paint(editor, SyntaxColors.Keyword, 18);                       // [link](url)
        Paint(editor, SyntaxColors.String, 19, 20, 21);                // `código` e blocos
    }
}
