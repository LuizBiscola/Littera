using Notas.Theme;
using ScintillaNET;

namespace Notas.UI;

internal sealed partial class GoToLineBox
{
    private void Go()
    {
        if (_activeEditor() is not { } editor)
            return;

        string[] parts = _input.Text.Split(':', ',');
        if (!int.TryParse(parts[0].Trim(), out int line))
            return;

        line = Math.Clamp(line, 1, editor.Lines.Count) - 1;
        int column = parts.Length > 1 && int.TryParse(parts[1].Trim(), out int c) ? Math.Max(0, c - 1) : 0;
        Line target = editor.Lines[line];
        target.EnsureVisible(); // linha dentro de um trecho recolhido: expande
        editor.GotoPosition(Math.Min(target.Position + column, target.EndPosition));
        editor.FirstVisibleLine = Math.Max(0, line - editor.LinesOnScreen / 2); // linha no meio da tela
        editor.Focus();
    }

    /// <summary>Centralizado no topo da área do editor.</summary>
    private void PlaceInParent()
    {
        if (Parent is null)
            return;
        Size = new Size(this.Px(300), this.Px(46));
        Location = new Point((Parent.ClientSize.Width - Width) / 2, this.Px(10));
        _input.Font = UiFonts.Get(13, DeviceDpi);
        _input.SetBounds(this.Px(18), (Height - _input.PreferredHeight) / 2, this.Px(150), _input.PreferredHeight);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Palette.Background);
        using (var border = new Pen(Palette.Border))
            g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);

        var box = new Rectangle(this.Px(8), this.Px(8), this.Px(170), Height - this.Px(16));
        Shapes.FillRounded(g, Palette.Accent, box, this.Px(6));
        box.Inflate(-1, -1);
        Shapes.FillRounded(g, Palette.Background, box, this.Px(5));

        int lines = _activeEditor()?.Lines.Count ?? 1;
        TextRenderer.DrawText(g, $"Ir para linha (1–{lines:N0})", UiFonts.Get(12, DeviceDpi),
            Rectangle.FromLTRB(box.Right + this.Px(10), 0, Width - this.Px(8), Height), Palette.TextMuted,
            TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
    }
}
