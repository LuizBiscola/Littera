using Notas.Editor.Changes;
using ScintillaNET;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private List<(int Line, bool Unsaved)>? _changedLines; // relido quando o texto muda ou o arquivo é salvo

    private List<(int Line, bool Unsaved)> ChangedLines(Scintilla editor) => _changedLines ??= ChangeMarkers.Lines(editor);

    /// <summary>Contagem da barra de status ("4 linhas não salvas · 2 salvas"); some com a opção desligada ou em arquivo enorme.</summary>
    private void RefreshChanges(Scintilla editor, bool small)
    {
        if (!_settings.Editor.ShowChanges || !small)
        {
            _status.SetChanges(0, 0);
            return;
        }

        List<(int Line, bool Unsaved)> lines = ChangedLines(editor);
        int unsaved = lines.Count(l => l.Unsaved);
        _status.SetChanges(unsaved, lines.Count - unsaved);
    }

    /// <summary>Alt+F5 e Shift+Alt+F5 (Alt+↓ já move a linha): vai ao começo do próximo trecho alterado.</summary>
    private void GoToChange(int step)
    {
        if (_boundEditor is not { } editor)
            return;
        var runs = ChangeNavigation.Runs(ChangedLines(editor).Select(l => l.Line));
        int line = ChangeNavigation.Next(runs, editor.CurrentLine, step);
        if (line < 0)
            return;

        editor.Lines[line].EnsureVisible(); // alteração dentro de um trecho recolhido
        editor.GotoPosition(editor.Lines[line].Position);
        editor.FirstVisibleLine = Math.Max(0, editor.Lines[line].DisplayIndex - editor.LinesOnScreen / 2);
        editor.Focus();
    }
}
