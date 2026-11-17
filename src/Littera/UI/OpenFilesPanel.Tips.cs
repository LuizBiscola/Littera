using Littera.Theme;
using Littera.UI.Controls;

namespace Littera.UI;

internal sealed partial class OpenFilesPanel
{
    private readonly System.Windows.Forms.Timer _tipTimer = new() { Interval = 500 };
    private DarkToolTip? _tip;
    private string? _tipText;

    /// <summary>Dica escura depois de 500 ms parado: botões dizem o que fazem e o atalho; a aba, onde está o arquivo.</summary>
    private void ScheduleTip(TabSlot? slot, TabButton button)
    {
        HideTip();
        _tipText = slot is not { } over ? null : button switch
        {
            TabButton.Pin => WithKeys(over.Pinned ? "Desafixar" : "Fixar aba", "fixarAba"),
            TabButton.Close => WithKeys("Fechar", "fecharAba"),
            _ when over.Document.Missing => $"Arquivo não encontrado: foi apagado ou movido\n{over.Document.Path}",
            _ => over.Document.Path ?? "Ainda não salvo",
        };
        if (_tipText is not null)
            _tipTimer.Start();
    }

    private string WithKeys(string text, string command) =>
        ShortcutText?.Invoke(command) is { Length: > 0 } keys ? $"{text}\t{keys}" : text;

    private void ShowTip()
    {
        _tipTimer.Stop();
        if (_tipText is null || _drag is not null)
            return;
        _tip ??= new DarkToolTip();
        Point mouse = PointToClient(Cursor.Position);
        _tip.ShowAt(_tipText, this, new Point(mouse.X + this.Px(10), mouse.Y + this.Px(20)));
    }

    private void HideTip()
    {
        _tipTimer.Stop();
        _tip?.Hide(this);
    }
}
