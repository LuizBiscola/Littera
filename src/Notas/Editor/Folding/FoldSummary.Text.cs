using System.Text;
using Notas.Editor.Formatting;

namespace Notas.Editor.Folding;

internal static partial class FoldSummary
{
    /// <summary>"dbo.pedidos" a partir de palavras e pontos seguidos.</summary>
    private static string NameAt(List<SqlToken> tokens, int at)
    {
        var name = new StringBuilder();
        for (int i = at; i < tokens.Count && (tokens[i].Kind is SqlTokenKind.Word or SqlTokenKind.Quoted || tokens[i].Is(".")); i++)
        {
            if (name.Length > 0 && !tokens[i].Is(".") && !tokens[i - 1].Is("."))
                break;
            name.Append(tokens[i].Text);
        }

        return name.Length > 0 ? name.ToString() : "…";
    }

    /// <summary>Tokens com espaço simples; "(" seguido de ")" vira "(…)".</summary>
    private static string Compact(List<SqlToken> list)
    {
        var text = new StringBuilder();
        for (int i = 0; i < list.Count; i++)
        {
            string piece = list[i].Is("(") && i + 1 < list.Count && list[i + 1].Is(")") ? "(…)" : list[i].Text;
            if (piece == "(…)")
                i++;
            bool glue = piece is "." or "," || (i > 0 && list[i - 1].Is("."));
            text.Append(glue || text.Length == 0 ? piece : " " + piece);
        }

        return text.ToString();
    }
}
