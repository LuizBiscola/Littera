using Littera.Theme;
using ScintillaNET;

namespace Littera.Editor.Languages;

internal sealed class XmlLanguage : Language
{
    public override string Name => "XML";

    public override string? Lexer => "xml";

    public override bool LexerFolds => true;

    public override string[] Extensions =>
        [".xml", ".xsd", ".xsl", ".xslt", ".config", ".csproj", ".props", ".targets", ".svg", ".xaml", ".resx", ".html", ".htm"];

    protected override void Configure(Scintilla editor)
    {
        editor.SetProperty("lexer.xml.allow.scripts", "0");

        Paint(editor, SyntaxColors.Tag, 1, 2, 11, 12, 13);   // tags, fechamento e <?xml ?>
        Paint(editor, SyntaxColors.Attribute, 3, 4);
        Paint(editor, SyntaxColors.Number, 5);
        Paint(editor, SyntaxColors.String, 6, 7);
        PaintItalic(editor, SyntaxColors.Comment, 9);
        Paint(editor, SyntaxColors.Variable, 10);            // &entidades;
        Paint(editor, SyntaxColors.Type, 17);                // CDATA
    }
}
