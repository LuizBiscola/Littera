using Notas.Theme;

namespace Notas.UI.Settings;

/// <summary>Linha de configuração do protótipo: rótulo e descrição à esquerda, controle à direita, divisória embaixo.</summary>
internal sealed class SettingRow : Control
{
    private readonly string _label;
    private readonly string? _description;
    private readonly Control? _input;

    public SettingRow(string label, string? description, Control? input)
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        _label = label;
        _description = description;
        _input = input;
        BackColor = Palette.Background;
        Height = this.Px(description is null ? 52 : description.Length > 60 ? 70 : 58);
        if (input is not null)
            Controls.Add(input);
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        if (_input is not null)
            _input.Location = new Point(Width - _input.Width, (Height - _input.Height) / 2);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(BackColor);
        int top = _description is null ? (Height - this.Px(18)) / 2 : this.Px(10);
        TextRenderer.DrawText(g, _label, UiFonts.Get(13, DeviceDpi), new Point(0, top), Palette.Text, TextFormatFlags.NoPadding);

        if (_description is not null)
        {
            int right = Width - (_input?.Width ?? 0) - this.Px(24);
            TextRenderer.DrawText(g, _description, UiFonts.Get(11.5f, DeviceDpi), new Rectangle(0, top + this.Px(20), right, this.Px(34)),
                Palette.TextMuted, TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);
        }

        using var line = new Pen(Palette.Border);
        g.DrawLine(line, 0, Height - 1, Width, Height - 1);
    }
}
