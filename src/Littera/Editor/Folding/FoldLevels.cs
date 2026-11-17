namespace Littera.Editor.Folding;

/// <summary>Converte trechos recolhíveis no nível de dobra de cada linha, no formato do Scintilla.</summary>
internal static class FoldLevels
{
    public const int Base = 0x400;
    public const int HeaderFlag = 0x2000;

    /// <summary>Base + quantos trechos contêm a linha; o cabeçalho leva a marca e fica no nível de fora.</summary>
    public static int[] From(int lineCount, IEnumerable<FoldRegion> regions)
    {
        var delta = new int[lineCount + 1];
        var headers = new bool[lineCount];
        foreach (FoldRegion region in regions)
        {
            if (region.Header < 0 || region.Last >= lineCount || region.Last <= region.Header)
                continue;
            delta[region.Header + 1]++;
            delta[region.Last + 1]--;
            headers[region.Header] = true;
        }

        var levels = new int[lineCount];
        int depth = 0;
        for (int line = 0; line < lineCount; line++)
        {
            depth += delta[line];
            levels[line] = (Base + depth) | (headers[line] ? HeaderFlag : 0);
        }

        return levels;
    }
}
