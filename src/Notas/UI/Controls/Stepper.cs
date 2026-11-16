using Notas.Theme;

namespace Notas.UI.Controls;

/// <summary>Contador "− 13 +" do protótipo (tamanho da fonte, espaçamento).</summary>
internal sealed class Stepper : Control
{
    private float _value;

    public Stepper()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        Size = new Size(this.Px(96), this.Px(32));
        Cursor = Cursors.Hand;
    }

    public event Action? ValueChanged;

    public float Minimum { get; init; }

    public float Maximum { get; init; } = 100;

    public float Increment { get; init; } = 1;

    public string Format { get; init; } = "0";

    public float Value
    {
        get => _value;
        set
        {
            float clamped = MathF.Round(Math.Clamp(value, Minimum, Maximum), 2);
            if (clamped == _value)
                return;
            _value = clamped;
            Invalidate();
            ValueChanged?.Invoke();
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.X < this.Px(30))
            Value -= Increment;
        else if (e.X > Width - this.Px(30))
            Value += Increment;
    }

}
