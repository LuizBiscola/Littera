using Notas.Theme;

namespace Notas.UI.Controls;

/// <summary>Dica da especificação: fundo escuro, texto claro e o atalho em cinza ("Fixar aba\tCtrl+Alt+P").</summary>
internal sealed partial class DarkToolTip : ToolTip
{
    private const TextFormatFlags Flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;

    private string _text = "";
    private int _dpi = 96;

    public DarkToolTip()
    {
        OwnerDraw = true;
        Popup += OnPopup;
        Draw += OnDraw;
    }

    public void ShowAt(string text, Control control, Point point)
    {
        _text = text;
        _dpi = control.DeviceDpi;
        Show(text, control, point);
    }

    private (string Text, string? Keys) Parts
    {
        get
        {
            int tab = _text.IndexOf('\t');
            return tab < 0 ? (_text, null) : (_text[..tab], _text[(tab + 1)..]);
        }
    }

    private int Px(int px) => (int)Math.Round(px * _dpi / 96.0);

    private void OnPopup(object? sender, PopupEventArgs e)
    {
        (string text, string? keys) = Parts;
        Font font = UiFonts.Get(11.5f, _dpi);
        Size size = TextRenderer.MeasureText(text, font, Size.Empty, Flags);
        if (keys is not null)
            size.Width += Px(10) + TextRenderer.MeasureText(keys, font, Size.Empty, Flags).Width;
        e.ToolTipSize = new Size(size.Width + Px(16), size.Height + Px(10));
    }
}
