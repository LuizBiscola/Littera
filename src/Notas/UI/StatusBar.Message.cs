namespace Notas.UI;

internal sealed partial class StatusBar
{
    private readonly System.Windows.Forms.Timer _messageTimer = new();
    private string? _message;

    /// <summary>Aviso curto no lugar do bloco e das ocorrências ("Aba fixada — desafixe para fechar"); some sozinho.</summary>
    public void ShowMessage(string text, int milliseconds = 2000)
    {
        _message = text;
        _messageTimer.Stop();
        _messageTimer.Interval = milliseconds;
        _messageTimer.Tick -= OnMessageExpired;
        _messageTimer.Tick += OnMessageExpired;
        _messageTimer.Start();
        Invalidate();
    }

    private void OnMessageExpired(object? sender, EventArgs e)
    {
        _messageTimer.Stop();
        _message = null;
        Invalidate();
    }
}
