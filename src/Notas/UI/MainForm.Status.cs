using Notas.Editor;
using Notas.Services;
using ScintillaNET;

namespace Notas.UI;

internal sealed partial class MainForm
{
    private Scintilla? _boundEditor;

    /// <summary>Cada aba tem seu editor: ao trocar, a barra passa a ouvir o novo.</summary>
    private void BindActiveEditor()
    {
        if (_boundEditor is not null)
        {
            _boundEditor.UpdateUI -= OnEditorUpdateUI;
            _boundEditor.Insert -= OnEditorTextChanged;
            _boundEditor.Delete -= OnEditorTextChanged;
        }

        _boundEditor = _documents.Active?.Editor;
        if (_boundEditor is not null)
        {
            _boundEditor.UpdateUI += OnEditorUpdateUI;
            _boundEditor.Insert += OnEditorTextChanged;
            _boundEditor.Delete += OnEditorTextChanged;
        }

        _blocks = null;
        _foldsStale = true;
        UpdateStatus();
    }

    /// <summary>Só seleção: o "Content" do UpdateUI também dispara com marcadores, e os destaques mexem neles.</summary>
    private void OnEditorUpdateUI(object? sender, UpdateUIEventArgs e)
    {
        if ((e.Change & UpdateChange.Selection) == 0)
            return;
        UpdatePosition();
        ScheduleHighlights(textChanged: false);
    }

    private void OnEditorTextChanged(object? sender, ModificationEventArgs e) => ScheduleHighlights(textChanged: true);

    private void UpdateStatus()
    {
        UpdatePosition();
        if (_documents.Active is not { } document)
            return;

        _status.SetSegment(StatusSegment.Indent, _settings.Editor.UseTabs ? "Tabulação" : $"Espaços: {_settings.Editor.IndentSize}");
        _status.SetSegment(StatusSegment.Encoding, document.Format.Encoding.Label());
        _status.SetSegment(StatusSegment.LineEnding, document.Format.LineEnding.Label());
        _status.SetSegment(StatusSegment.Language, document.Language.Name);
        if (document.IsLoading)
            _status.SetBlock("Carregando…");
        else
            ScheduleHighlights(textChanged: false);
    }

    private void UpdatePosition()
    {
        if (_boundEditor is null)
            return;

        int position = _boundEditor.CurrentPosition;
        _status.SetPosition(_boundEditor.LineFromPosition(position) + 1, _boundEditor.GetColumn(position) + 1);
    }
}
