using System.Drawing.Drawing2D;
using Notas.Theme;

namespace Notas.UI.Controls;

/// <summary>Chave liga/desliga do protótipo (azul quando ligada).</summary>
internal sealed class ToggleSwitch : Control
{
    private bool _checked;

    public ToggleSwitch()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
        Size = new Size(this.Px(36), this.Px(20));
        Cursor = Cursors.Hand;
    }

    public event Action? CheckedChanged;

    public bool Checked
    {
        get => _checked;
        set
        {
            if (_checked == value)
                return;
            _checked = value;
            Invalidate();
            CheckedChanged?.Invoke();
        }
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        Checked = !Checked;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        Shapes.FillRounded(g, _checked ? Palette.Accent : Palette.Hex(0xD3D6DB), new Rectangle(0, 0, Width - 1, Height - 1), Height / 2);

        int knob = Height - this.Px(6);
        int x = _checked ? Width - knob - this.Px(4) : this.Px(3);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.FillEllipse(Brushes.White, x, this.Px(3), knob, knob);
    }
}
