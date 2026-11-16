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
        Size = new Size(this.Px(76) * options.Length + this.Px(6), this.Px(30));
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

}
