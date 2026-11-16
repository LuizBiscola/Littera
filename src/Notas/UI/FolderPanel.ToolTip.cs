namespace Notas.UI;

internal sealed partial class FolderPanel
{
    private ToolTip? _toolTip;
    private int _toolTipIndex = -1;

    protected override void OnMouseHover(EventArgs e)
    {
        base.OnMouseHover(e);
        if (_hoverIndex == _toolTipIndex)
            return;
        _toolTipIndex = _hoverIndex;
        _toolTip ??= new ToolTip();
        _toolTip.SetToolTip(this, _hoverIndex >= 0 ? _rows[_hoverIndex].Entry.Path : HoverButtonText());
    }

    private string? HoverButtonText() =>
        _hoverButton == RefreshButton ? "Atualizar" : _hoverButton == CollapseButton ? "Recolher tudo"
        : _hoverButton == CloseButton ? "Fechar pasta" : null;
}
