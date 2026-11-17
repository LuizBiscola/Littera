using Notas.Editor.Folding;
using Notas.Theme;
using ScintillaNET;

namespace Notas.UI;

/// <summary>Prévia do trecho recolhido (protótipo "Blocos recolhidos"): aparece ao parar o mouse sobre o resumo.</summary>
internal sealed partial class FoldPreview : Control
{
    private const int PreviewWidth = 500;
    private const int HeaderHeight = 38;
    private const int FooterHeight = 30;
    private const int LineHeight = 20;
    private const int MaxLines = 14;

    private Scintilla? _editor;
    private int _first, _last;
    private string _title = "", _subtitle = "";

    public FoldPreview()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        Visible = false;
    }

    /// <summary>Botão "Expandir": quem abriu expande o cabeçalho (e atualiza a barra de status).</summary>
    public event Action<Scintilla, int>? ExpandRequested;

    /// <summary>Atalho mostrado no rodapé (o configurado para "expandirBloco").</summary>
    public string? Shortcut { get; set; }

    public int Header { get; private set; } = -1;

    /// <summary>Mostra as linhas escondidas pelo cabeçalho; <paramref name="anchor"/> é o canto onde o cartão encosta.</summary>
    public void Show(Scintilla editor, int header, Point anchor, bool above)
    {
        _editor = editor;
        Header = header;
        _first = header + 1;
        _last = EditorFolding.LastChild(editor, header);
        _title = _first == _last ? $"Linha {_first + 1}" : $"Linhas {_first + 1}–{_last + 1}";
        _subtitle = editor.Lines[header].Text.Trim();
        editor.Colorize(editor.Lines[_first].Position, editor.Lines[_last].EndPosition); // cores das linhas que nunca foram desenhadas

        int rows = Math.Min(_last - _first + 1, MaxLines) + (_last - _first + 1 > MaxLines ? 1 : 0);
        Size = new Size(this.Px(PreviewWidth), this.Px(HeaderHeight + FooterHeight + 12) + rows * this.Px(LineHeight));
        Location = new Point(anchor.X, above ? anchor.Y - Height : anchor.Y);
        Visible = true;
        BringToFront();
        Invalidate();
    }

    public void Dismiss()
    {
        Visible = false;
        Header = -1;
    }
}
