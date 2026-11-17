namespace Notas.UI;

/// <summary>Ícone da janela: o mesmo .ico do exe (Assets\Notas.ico), embutido com todos os tamanhos.</summary>
internal static class AppIcon
{
    /// <summary>O WinForms escolhe no .ico o tamanho grande (Alt+Tab) e o pequeno (barra de tarefas) pelo DPI.</summary>
    public static Icon? Load()
    {
        using Stream? stream = typeof(AppIcon).Assembly.GetManifestResourceStream("Notas.ico");
        return stream is null ? null : new Icon(stream);
    }
}
