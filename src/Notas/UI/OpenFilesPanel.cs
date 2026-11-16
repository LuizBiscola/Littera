using Notas.Editor;
using Notas.Theme;

namespace Notas.UI;

/// <summary>Painel "ABERTOS" do protótipo: abas verticais com ponto de "não salvo" e botão de fechar.</summary>
internal sealed partial class OpenFilesPanel : Control
{
    private const int HeaderHeight = 48;
    private const int FooterHeight = 35;
    private const int ItemHeight = 32;
    private const int ItemGap = 2;

    private readonly DocumentManager _documents;

    public OpenFilesPanel(DocumentManager documents)
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        _documents = documents;
        _documents.Changed += Invalidate;
        Dock = DockStyle.Left;
        Width = this.Px(236);
    }

    private Rectangle NewButtonBounds => new(Width - this.Px(34), this.Px(14), this.Px(24), this.Px(24));

    private Rectangle ItemBounds(int index)
    {
        int top = this.Px(HeaderHeight + 2) + index * this.Px(ItemHeight + ItemGap) - _scroll;
        return new Rectangle(this.Px(8), top, Width - this.Px(16), this.Px(ItemHeight));
    }

    private Rectangle CloseBounds(Rectangle item) =>
        new(item.Right - this.Px(26), item.Top + this.Px(6), this.Px(20), this.Px(20));

    private Rectangle ListArea => Rectangle.FromLTRB(0, this.Px(HeaderHeight), Width, Height - this.Px(FooterHeight));

    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        Width = this.Px(236);
    }
}
