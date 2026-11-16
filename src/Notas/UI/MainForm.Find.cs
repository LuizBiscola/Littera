using Notas.Editor.Highlights;
using Notas.Editor.Search;
using Notas.UI.Find;

namespace Notas.UI;

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
                _findPanel = new FindPanel(() => _documents.Active?.Editor) { BlockRange = CurrentBlockRange };
                _editorHost.Controls.Add(_findPanel);
            }

            return _findPanel;
        }
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
