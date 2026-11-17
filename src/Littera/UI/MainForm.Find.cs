using Littera.Editor.Highlights;
using Littera.Editor.Search;
using Littera.UI.Find;

namespace Littera.UI;

internal sealed partial class MainForm
{
    private FindPanel? _findPanel;

    /// <summary>Criado no primeiro uso: nada da busca entra na inicialização.</summary>
    private FindPanel FindPanel
    {
        get
        {
            if (_findPanel is null)
            {
                _findPanel = new FindPanel(() => _documents.Active?.Editor) { BlockRange = CurrentBlockRange, Files = this };
                _editorHost.Controls.Add(_findPanel);
                _documents.ActiveChanged += KeepFloatingOnTop;
            }

            return _findPanel;
        }
    }

    private GoToLineBox? _goToLine;

    /// <summary>Ctrl+G; como a busca, só é criada no primeiro uso.</summary>
    private void ShowGoToLine()
    {
        if (_goToLine is null)
        {
            _goToLine = new GoToLineBox(() => _documents.Active?.Editor);
            _editorHost.Controls.Add(_goToLine);
            _documents.ActiveChanged += KeepFloatingOnTop;
        }

        _goToLine.Open();
    }

    /// <summary>Trocar de aba traz o editor para a frente; busca e "ir para linha" abertas continuam por cima.</summary>
    private void KeepFloatingOnTop()
    {
        foreach (Control floating in new Control?[] { _findPanel, _goToLine }.OfType<Control>().Where(c => c.Visible))
            floating.BringToFront();
    }

    /// <summary>Faixa de texto do bloco do cursor (escopo "Bloco atual" da busca).</summary>
    private TextRange? CurrentBlockRange()
    {
        if (_boundEditor is not { } editor || _blocks is null)
            return null;

        int line = editor.CurrentLine;
        LineBlock block = _blocks.Find(b => b.Contains(line));
        if (!block.Contains(line))
            return null;
        return new TextRange(editor.Lines[block.FirstLine].Position, editor.Lines[block.LastLine].EndPosition);
    }
}
