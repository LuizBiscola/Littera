using Notas.Theme;
using ScintillaNET;

namespace Notas.UI;

internal sealed partial class FoldPreview
{
    /// <summary>Linhas escondidas com número e as cores do realce do próprio editor.</summary>
    private void PaintCode(Graphics g, Rectangle area)
    {
        if (_editor is not { } editor)
            return;
        using (var back = new SolidBrush(Palette.CodePreview))
            g.FillRectangle(back, area);

        Style baseStyle = editor.Styles[Style.Default];
        using var font = new Font(baseStyle.Font, 12f * DeviceDpi / 96f, GraphicsUnit.Pixel); // 12 px, como no protótipo
        int numbers = this.Px(36), y = area.Top + this.Px(6), height = this.Px(LineHeight);
        int count = Math.Min(_last - _first + 1, MaxLines);
        for (int i = 0; i < count; i++, y += height)
        {
            int line = _first + i;
            TextRenderer.DrawText(g, (line + 1).ToString(), font, new Rectangle(area.Left, y, numbers - this.Px(10), height),
                Palette.LineNumber, Line | TextFormatFlags.Right);
            PaintLine(g, editor, line, font, new Rectangle(area.Left + numbers, y, area.Width - numbers - this.Px(8), height));
        }

        if (_last - _first + 1 > MaxLines)
            TextRenderer.DrawText(g, $"… mais {_last - _first + 1 - MaxLines} linhas", font,
                new Rectangle(area.Left + numbers, y, area.Width - numbers, height), Palette.TextFaint, Line);
    }

    /// <summary>Um pedaço por estilo (palavra-chave, string, comentário…), na cor do estilo.</summary>
    private void PaintLine(Graphics g, Scintilla editor, int line, Font font, Rectangle bounds)
    {
        string text = editor.Lines[line].Text.TrimEnd('\r', '\n');
        int start = editor.Lines[line].Position, x = bounds.X;
        for (int i = 0; i < text.Length && x < bounds.Right;)
        {
            int style = editor.GetStyleAt(start + i);
            int j = i + 1;
            while (j < text.Length && editor.GetStyleAt(start + j) == style)
                j++;
            string run = text[i..j].Replace("\t", new string(' ', editor.TabWidth));
            TextRenderer.DrawText(g, run, font, new Rectangle(x, bounds.Y, bounds.Right - x, bounds.Height), editor.Styles[style].ForeColor, Line);
            x += TextRenderer.MeasureText(g, run, font, Size.Empty, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width;
            i = j;
        }
    }
}
