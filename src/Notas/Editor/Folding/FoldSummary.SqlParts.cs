using Notas.Editor.Formatting;

namespace Notas.Editor.Folding;

internal static partial class FoldSummary
{
    /// <summary>"SELECT 4 colunas" (vírgulas de primeiro nível + 1) ou "SELECT *".</summary>
    private static string SelectPart(List<SqlToken> tokens, int at)
    {
        List<SqlToken> list = Clause(tokens, at);
        int columns = 1 + list.Count(t => t.Is(","));
        if (columns == 1 && list.Exists(t => t.Is("*")))
            return "SELECT *";
        return columns == 1 ? "SELECT 1 coluna" : $"SELECT {columns} colunas";
    }

    /// <summary>Uma condição aparece por extenso ("WHERE id IN (…)"); mais de uma vira "WHERE 3 filtros".</summary>
    private static string ConditionPart(List<SqlToken> tokens, int at)
    {
        List<SqlToken> list = Clause(tokens, at);
        int between = list.Count(t => t.Is("BETWEEN"));
        int filters = 1 + list.Count(t => t.Is("AND") || t.Is("OR")) - between;
        string keyword = tokens[at].Text.ToUpperInvariant();
        return filters > 1 ? $"{keyword} {filters} filtros" : $"{keyword} {Shorten(Compact(list), 32)}";
    }

    /// <summary>"SET segmento", ou "SET segmento +2" com mais colunas.</summary>
    private static string SetPart(List<SqlToken> tokens, int at)
    {
        List<SqlToken> list = Clause(tokens, at);
        int more = list.Count(t => t.Is(","));
        return $"SET {NameAt(tokens, at + 1)}{(more > 0 ? $" +{more}" : "")}";
    }
}
