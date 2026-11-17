using Littera.Theme;
using ScintillaNET;

namespace Littera.UI;

/// <summary>Ctrl+G: caixa no topo do editor; aceita "linha" ou "linha:coluna".</summary>
internal sealed partial class GoToLineBox : Control
{
    private readonly TextBox _input = new()
    {
        BorderStyle = System.Windows.Forms.BorderStyle.None, BackColor = Palette.Background, ForeColor = Palette.Text,
    };

    private readonly Func<Scintilla?> _activeEditor;

    public GoToLineBox(Func<Scintilla?> activeEditor)
    {
        SuspendLayout();
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        _activeEditor = activeEditor;
        Visible = false;
        Controls.Add(_input);
        _input.KeyDown += OnInputKeyDown;
        _input.LostFocus += (_, _) => Visible = false;
        ResumeLayout(false);
    }

    public void Open()
    {
        if (_activeEditor() is not { } editor)
            return;

        _input.Text = (editor.CurrentLine + 1).ToString();
        PlaceInParent();
        Visible = true;
        BringToFront();
        _input.Focus();
        _input.SelectAll();
        Invalidate();
    }

    private void OnInputKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
            _activeEditor()?.Focus();
        else if (e.KeyCode == Keys.Enter)
            Go();
        else
            return;
        e.SuppressKeyPress = true;
    }
}
