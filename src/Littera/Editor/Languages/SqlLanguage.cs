using Notas.Theme;
using ScintillaNET;

namespace Notas.Editor.Languages;

/// <summary>
/// SQL com o lexer "mssql" da Lexilla: no ANSI realça igual ao "sql" e ainda distingue
/// @variáveis e @@globais do T-SQL (comparação feita na Fase 0).
/// </summary>
internal sealed class SqlLanguage : Language
{
    public override string Name => "SQL";

    public override string? Lexer => "mssql";

    public override string? LineComment => "--";

    public override string[] Extensions => [".sql", ".ddl", ".dml", ".tsql", ".pgsql", ".prc", ".fnc", ".vw"];

    protected override void Configure(Scintilla editor)
    {
        editor.SetKeywords(0, SqlKeywords.Keywords);
        editor.SetKeywords(1, SqlKeywords.DataTypes);
        editor.SetKeywords(4, SqlKeywords.Functions);

        PaintItalic(editor, SyntaxColors.Comment, 1, 2);   // /* */ e --
        Paint(editor, SyntaxColors.Number, 3);
        Paint(editor, SyntaxColors.String, 4);
        Paint(editor, SyntaxColors.Variable, 7, 12);       // @variavel e @@global
        Paint(editor, SyntaxColors.Keyword, 9);
        Paint(editor, SyntaxColors.Type, 10, 15);
        Paint(editor, SyntaxColors.Function, 13);
    }
}
