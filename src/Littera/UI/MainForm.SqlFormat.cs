using Littera.Editor.Formatting;
using Littera.Services;

namespace Littera.UI;

internal sealed partial class MainForm
{
    /// <summary>Ctrl+Shift+F: formata a seleção (ou o arquivo todo) num único passo de desfazer.</summary>
    private void FormatSql()
    {
        if (_documents.Active is not { IsLoading: false, Editor: { } editor } document)
            return;

        var options = new SqlFormatOptions(
            _settings.SqlFormat.KeywordCase,
            _settings.SqlFormat.OneColumnPerLine,
            _settings.SqlFormat.BreakConditions,
            _settings.Editor.UseTabs ? "\t" : new string(' ', _settings.Editor.IndentSize),
            document.Format.LineEnding switch { LineEnding.Lf => "\n", LineEnding.Cr => "\r", _ => "\r\n" });

        bool selection = editor.SelectionStart != editor.SelectionEnd;
        int start = selection ? editor.SelectionStart : 0;
        int end = selection ? editor.SelectionEnd : editor.TextLength;
        string original = editor.GetTextRange(start, end - start);
        string formatted = SqlFormatter.Format(original, options);
        if (formatted == original)
            return;

        int firstVisible = editor.FirstVisibleLine;
        editor.SetTargetRange(start, end);
        editor.ReplaceTarget(formatted);
        if (selection)
            editor.SetSelection(start + formatted.Length, start);
        else
            editor.GotoPosition(Math.Min(editor.CurrentPosition, editor.TextLength));
        editor.FirstVisibleLine = firstVisible;
    }
}
