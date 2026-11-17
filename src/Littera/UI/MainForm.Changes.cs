using Littera.Editor;
using Littera.Editor.Changes;
using ScintillaNET;

namespace Littera.UI;

internal sealed partial class MainForm
{
    private static readonly ChangeBaseline Untitled = new([""]); // arquivo novo compara com o vazio

    private ChangeSet? _changeSet; // comparação do editor ativo com o disco
    private bool _changesStale = true;
    private int _changeVersion; // muda a cada edição e ao salvar: comparação atrasada é descartada

    /// <summary>Compara de novo quando o texto muda ou o arquivo é salvo, fora da thread da interface.</summary>
    private void RefreshChanges(Scintilla editor, OpenDocument document, bool small, Func<string> text)
    {
        if (!_changesStale)
            return;
        _changesStale = false;
        ChangeBaseline? baseline = document.Baseline ?? (document.Path is null ? Untitled : null);
        if (!_settings.Editor.ShowChanges || !small || baseline is null)
        {
            ShowChanges(editor, null);
            return;
        }

        _ = CompareAsync(editor, baseline, ChangeBaseline.Lines(text()), _changeVersion);
    }

    private async Task CompareAsync(Scintilla editor, ChangeBaseline baseline, string[] current, int version)
    {
        ChangeSet? changes = await Task.Run(() => ChangeSet.Compute(baseline.Original, baseline.Saved, current));
        if (version == _changeVersion && _boundEditor == editor)
            ShowChanges(editor, changes);
    }

    private void ShowChanges(Scintilla editor, ChangeSet? changes)
    {
        _changeSet = changes;
        _hoverHunk = null;
        ChangeMarkers.Apply(editor, changes);
        _status.SetChanges(changes?.UnsavedLines.Count ?? 0, changes?.SavedLines.Count ?? 0);
    }

    /// <summary>Texto mudou, arquivo foi salvo ou trocou a aba: a comparação precisa ser refeita.</summary>
    private void InvalidateChanges()
    {
        _changesStale = true;
        _changeVersion++;
    }

    /// <summary>Alt+F5 e Shift+Alt+F5 (Alt+↓ já move a linha): vai ao começo do próximo trecho alterado.</summary>
    private void GoToChange(int step)
    {
        if (_boundEditor is not { } editor || _changeSet is not { } changes)
            return;
        var runs = ChangeNavigation.Runs(changes.UnsavedLines.Union(changes.SavedLines).Order());
        int line = ChangeNavigation.Next(runs, editor.CurrentLine, step);
        if (line < 0)
            return;

        editor.Lines[line].EnsureVisible(); // alteração dentro de um trecho recolhido
        editor.GotoPosition(editor.Lines[line].Position);
        editor.FirstVisibleLine = Math.Max(0, editor.Lines[line].DisplayIndex - editor.LinesOnScreen / 2);
        editor.Focus();
    }
}
