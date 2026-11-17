namespace Littera.Services;

/// <summary>Fim de linha predominante no começo do texto; sem quebras, usa o padrão do Windows (CRLF).</summary>
internal static class EolDetector
{
    public static LineEnding Detect(ReadOnlySpan<char> text)
    {
        int crlf = 0, lf = 0, cr = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                lf++;
            }
            else if (text[i] == '\r')
            {
                if (i + 1 < text.Length && text[i + 1] == '\n')
                {
                    crlf++;
                    i++;
                }
                else
                {
                    cr++;
                }
            }
        }

        if (lf > crlf && lf >= cr)
            return LineEnding.Lf;
        return cr > crlf && cr > lf ? LineEnding.Cr : LineEnding.CrLf;
    }
}
