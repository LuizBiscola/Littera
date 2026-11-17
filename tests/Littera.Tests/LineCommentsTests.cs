using Littera.Editor.Editing;

namespace Littera.Tests;

public class LineCommentsTests
{
    private static string[] Apply(string[] lines, string prefix)
    {
        string[] result = [.. lines];
        foreach (LineEdit edit in LineComments.Toggle(lines, prefix))
        {
            string line = result[edit.Line];
            result[edit.Line] = line[..edit.Column] + edit.Insert + line[(edit.Column + edit.Remove)..];
        }

        return result;
    }

    [Fact]
    public void Comenta_no_menor_recuo_e_pula_linhas_em_branco()
    {
        string[] lines = ["    SELECT a,", "", "           b", "    FROM t"];
        Assert.Equal(["    -- SELECT a,", "", "    --        b", "    -- FROM t"], Apply(lines, "--"));
    }

    [Fact]
    public void Descomenta_quando_todas_ja_estao_comentadas()
    {
        string[] lines = ["  -- SELECT 1", "  --FROM t"];
        Assert.Equal(["  SELECT 1", "  FROM t"], Apply(lines, "--"));
    }

    [Fact]
    public void Mistura_de_comentadas_e_nao_comentadas_comenta_todas()
    {
        Assert.Equal(["# # a", "# b"], Apply(["# a", "b"], "#"));
    }

    [Fact]
    public void So_linhas_em_branco_nao_mudam_nada()
    {
        Assert.Empty(LineComments.Toggle(["", "   "], "//"));
    }
}
