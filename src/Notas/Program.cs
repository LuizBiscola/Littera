using Notas.Editor;
using Notas.Services;
using Notas.Settings;
using Notas.UI;

namespace Notas;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        StartupMetrics.Init();
        StartupMetrics.Mark("t_main");

        // Precisa rodar antes de qualquer acesso ao tipo Scintilla.
        NativeLibraryBootstrap.Configure();
        NativeLibraryBootstrap.PreloadInBackground();
        StartupMetrics.Mark("t_bootstrap");

        ApplicationConfiguration.Initialize();
        AppSettings settings = SettingsStore.LoadSettings();
        StartupMetrics.Mark("t_settings");
        var form = new MainForm(settings, args);
        StartupMetrics.Mark("t_form");
        Application.Run(form);
    }
}
