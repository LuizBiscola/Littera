using Notas.Theme;

namespace Notas.UI.Controls;

/// <summary>Escolha de fonte no estilo do Stepper: digita com autocompletar ou escolhe na lista da seta.</summary>
internal sealed partial class FontBox : Control
{
    private readonly TextBox _text = new() { BorderStyle = BorderStyle.None, BackColor = Palette.Background, ForeColor = Palette.Text };
    private readonly string[] _fonts;

    public FontBox(string[] fonts)
    {
        SuspendLayout();
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        _fonts = fonts;
        // Autocompletar ligado antes da janela da caixa existir falha ao criá-la (erro 1400): liga depois.
        _text.HandleCreated += (_, _) => BeginInvoke(EnableAutoComplete);
        _text.TextChanged += (_, _) => ValueChanged?.Invoke();
        _text.GotFocus += (_, _) => Invalidate(); // borda azul com o foco
        _text.LostFocus += (_, _) => Invalidate();
        Controls.Add(_text);
        Size = new Size(this.Px(240), this.Px(32));
        ResumeLayout(false);
    }

    public event Action? ValueChanged;

    public string Value
    {
        get => _text.Text;
        set => _text.Text = value;
    }

    private void EnableAutoComplete()
    {
        if (_text.AutoCompleteMode != AutoCompleteMode.None || _text.IsDisposed)
            return;
        _text.AutoCompleteCustomSource.AddRange(_fonts);
        _text.AutoCompleteSource = AutoCompleteSource.CustomSource;
        _text.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
    }

    private Rectangle ArrowBounds => new(Width - this.Px(30), 0, this.Px(30), Height);

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        _text.Font = UiFonts.Get(13, DeviceDpi);
        int height = _text.PreferredHeight;
        _text.SetBounds(this.Px(10), (Height - height) / 2, ArrowBounds.Left - this.Px(14), height);
    }
}
