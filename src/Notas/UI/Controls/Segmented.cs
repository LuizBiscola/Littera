using Notas.Theme;

namespace Notas.UI.Controls;

/// <summary>Seletor "Espaços 2 | Espaços 4 | Tab" do protótipo: fundo cinza, opção escolhida em branco.</summary>
internal sealed class Segmented : Control
{
    private readonly string[] _options;
    private int _selected;

    public Segmented(params string[] options)
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        _options = options;
        int each = Math.Max(this.Px(76), options.Max(o => TextRenderer.MeasureText(o, UiFonts.Get(12.5f, DeviceDpi)).Width) + this.Px(20));
        Size = new Size(each * options.Length + this.Px(6), this.Px(30));
        Cursor = Cursors.Hand;
    }

    public event Action? SelectedIndexChanged;

    public int SelectedIndex
    {
        get => _selected;
        set
        {
            if (_selected == value)
                return;
            _selected = value;
            Invalidate();
            SelectedIndexChanged?.Invoke();
        }
    }

    private Rectangle Segment(int i)
    {
        int width = (Width - this.Px(6)) / _options.Length;
        return new Rectangle(this.Px(3) + i * width, this.Px(3), width, Height - this.Px(6));
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        for (int i = 0; i < _options.Length; i++)
        {
            if (Segment(i).Contains(e.Location))
                SelectedIndex = i;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Parent?.BackColor ?? Palette.Background);
        Shapes.FillRounded(g, Palette.ItemHover, new Rectangle(0, 0, Width, Height), this.Px(6));
        for (int i = 0; i < _options.Length; i++)
        {
            Rectangle segment = Segment(i);
            if (i == _selected)
                Shapes.FillRounded(g, Palette.Background, segment, this.Px(5));
            TextRenderer.DrawText(g, _options[i], UiFonts.Get(12.5f, DeviceDpi), segment, i == _selected ? Palette.Text : Palette.TextMuted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }
}
