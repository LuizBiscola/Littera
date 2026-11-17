namespace Littera.UI.Commands;

internal sealed partial class CommandPalette
{
    /// <summary>Setas e Page Up/Down andam pela lista; Enter escolhe. Esc fica com a janela (fecha a paleta).</summary>
    private void OnQueryKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Down: Step(1); break;
            case Keys.Up: Step(-1); break;
            case Keys.PageDown: Step(MaxRows); break;
            case Keys.PageUp: Step(-MaxRows); break;
            case Keys.Enter: Choose(_selected); break;
            default: return;
        }

        e.SuppressKeyPress = true;
    }

    private void Step(int step)
    {
        if (_shown.Count == 0)
            return;
        _selected = Math.Clamp(_selected + step, 0, _shown.Count - 1);
        if (_selected < _top)
            _top = _selected;
        else if (_selected >= _top + MaxRows)
            _top = _selected - MaxRows + 1;
        Invalidate();
    }

    private void Choose(int index)
    {
        if (index >= 0 && index < _shown.Count)
            Chosen?.Invoke(_shown[index]);
    }
}
