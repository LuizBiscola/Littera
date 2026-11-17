namespace Littera.UI;

internal sealed partial class MainForm
{
    /// <summary>Volta posição e tamanho salvos, se ainda couberem em algum monitor.</summary>
    private void RestoreWindowPlacement()
    {
        var saved = new Rectangle(_settings.Window.X, _settings.Window.Y, _settings.Window.Width, _settings.Window.Height);
        bool visible = saved.Width > 0 && saved.Height > 0
            && Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(Rectangle.Inflate(saved, -100, -60)));
        if (!visible)
            return;

        StartPosition = FormStartPosition.Manual;
        Bounds = saved;
        if (_settings.Window.Maximized)
            WindowState = FormWindowState.Maximized;
    }

    /// <summary>Maximizada, guarda o tamanho "normal" para quando o usuário restaurar.</summary>
    private void SaveWindowPlacement()
    {
        if (WindowState == FormWindowState.Minimized)
            return;

        Rectangle bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
        _settings.Window.X = bounds.X;
        _settings.Window.Y = bounds.Y;
        _settings.Window.Width = bounds.Width;
        _settings.Window.Height = bounds.Height;
        _settings.Window.Maximized = WindowState == FormWindowState.Maximized;
    }
}
