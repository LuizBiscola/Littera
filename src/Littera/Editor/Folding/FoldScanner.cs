using Littera.Editor.Highlights;

namespace Littera.Editor.Folding;

/// <summary>Trecho recolhível: a linha Header continua visível; Header+1 até Last somem.</summary>
internal readonly record struct FoldRegion(int Header, int Last);

/// <summary>Onde dá para recolher: parênteses de várias linhas, instruções, comentários seguidos e parágrafos.</summary>
internal static partial class FoldScanner
{
    public static List<FoldRegion> Sql(string text, List<LineBlock> statements)
    {
        Dictionary<int, int> parens = Brackets(text);
        var regions = parens.Select(p => new FoldRegion(p.Key, p.Value)).ToList();
        foreach (LineBlock statement in statements)
        {
            // "WITH x AS (" abre parênteses na primeira linha: a instrução recolhe a partir de onde eles fecham.
            int header = statement.FirstLine;
            while (parens.TryGetValue(header, out int closes))
                header = closes + 1;
            if (statement.LastLine > header)
                regions.Add(new FoldRegion(header, statement.LastLine));
        }

        regions.AddRange(CommentRuns(text));
        return regions;
    }

    public static List<FoldRegion> Paragraphs(List<LineBlock> paragraphs) =>
        [.. paragraphs.Where(p => p.LastLine > p.FirstLine).Select(p => new FoldRegion(p.FirstLine, p.LastLine))];
}
