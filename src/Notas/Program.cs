using Notas.Editor;
using Notas.Services;
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
        var form = new MainForm(args);
        StartupMetrics.Mark("t_form");
        Application.Run(form);
    }
}
