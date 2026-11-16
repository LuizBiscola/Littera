using Notas.Editor;

namespace Notas.UI;

internal sealed partial class OpenFilesPanel
{
    private ToolTip? _toolTip;
    private int _toolTipIndex = -1;

    /// <summary>Fechar pela aba (clique no X ou botão do meio): quem trata pergunta se quer salvar.</summary>
    public event Action<OpenDocument>? CloseRequested;

    public event Action? NewRequested;

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button == MouseButtons.Left && NewButtonBounds.Contains(e.Location))
        {
            NewRequested?.Invoke();
            return;
        }

        int index = IndexAt(e.Location);
        if (index < 0)
            return;

        OpenDocument document = _documents.Documents[index];
        bool onClose = CloseBounds(ItemBounds(index)).Contains(e.Location);
        if (e.Button == MouseButtons.Middle || (e.Button == MouseButtons.Left && onClose))
            CloseRequested?.Invoke(document);
        else if (e.Button == MouseButtons.Left)
            _documents.Activate(document);
    }

    /// <summary>Caminho completo ao passar o mouse sobre a aba.</summary>
    protected override void OnMouseHover(EventArgs e)
    {
        base.OnMouseHover(e);
        if (_hoverIndex == _toolTipIndex || _hoverIndex >= _documents.Documents.Count)
            return;

        _toolTipIndex = _hoverIndex;
        _toolTip ??= new ToolTip();
        _toolTip.SetToolTip(this, _hoverIndex >= 0 ? _documents.Documents[_hoverIndex].Path ?? "Ainda não salvo" : null);
    }
}
