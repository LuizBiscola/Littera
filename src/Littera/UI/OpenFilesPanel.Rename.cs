using Notas.Editor;
using Notas.Theme;

namespace Notas.UI;

internal sealed partial class OpenFilesPanel
{
    private TextBox? _renameBox;
    private OpenDocument? _renaming;

    /// <summary>Novo nome digitado (só o nome, sem pasta): quem trata renomeia o arquivo no disco.</summary>
    public event Action<OpenDocument, string>? RenameRequested;

    /// <summary>F2 / "Renomear arquivo…": caixa no lugar do nome, com o nome selecionado sem a extensão.</summary>
    public void BeginRename(OpenDocument document)
    {
        if (document.Path is not { } path || FindSlot(document) is not { } slot)
            return;

        _renaming = document;
        _renameBox ??= CreateRenameBox();
        _renameBox.Font = UiFonts.Get(13, DeviceDpi);
        Rectangle row = slot.Bounds;
        _renameBox.SetBounds(row.X + this.Px(29), row.Y + (row.Height - _renameBox.PreferredHeight) / 2,
            row.Width - this.Px(37), _renameBox.PreferredHeight);
        _renameBox.Text = Path.GetFileName(path);
        _renameBox.Visible = true;
        _renameBox.Focus();
        _renameBox.Select(0, Path.GetFileNameWithoutExtension(path).Length);
    }

    private TextBox CreateRenameBox()
    {
        var box = new TextBox { BorderStyle = BorderStyle.FixedSingle, Visible = false };
        box.KeyDown += (_, e) =>
        {
            if (e.KeyCode is not (Keys.Enter or Keys.Escape))
                return;
            FinishRename(commit: e.KeyCode == Keys.Enter);
            e.SuppressKeyPress = true;
        };
        box.LostFocus += (_, _) => FinishRename(commit: false); // clicou fora: desiste, como o Esc
        Controls.Add(box);
        return box;
    }

    private void FinishRename(bool commit)
    {
        if (_renameBox is not { Visible: true } box || _renaming is not { } document)
            return;
        _renaming = null;
        box.Visible = false;
        string name = box.Text.Trim();
        if (commit && name.Length > 0 && name != Path.GetFileName(document.Path))
            RenameRequested?.Invoke(document, name);
    }
}
