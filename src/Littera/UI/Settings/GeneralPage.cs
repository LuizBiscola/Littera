using Notas.Settings;
using Notas.UI.Controls;

namespace Notas.UI.Settings;

/// <summary>Página "Geral": janela e renderização.</summary>
internal sealed class GeneralPage : SettingsPage
{
    public override string Name => "Geral";

    public override char Icon => ''; // "Equalizer" na Segoe Fluent

    public override string Subtitle => "Janela e desenho do texto.";

    public override IEnumerable<Control> Build(AppSettings draft)
    {
        yield return Section("JANELA");
        yield return Toggle("Mostrar abas abertas", "Painel à esquerda (Ctrl+B mostra e esconde a qualquer momento).",
            draft.Window.ShowOpenFiles, v => draft.Window.ShowOpenFiles = v);
        yield return Toggle("Reabrir sessão", $"Ao abrir o {Product.Name}, volta com as abas da última vez. Fixadas e abas com alterações não salvas voltam sempre.",
            draft.Window.ReopenSession, v => draft.Window.ReopenSession = v);

        yield return Section("DESENHO DO TEXTO");
        var renderer = new Segmented("GDI", "DirectWrite") { SelectedIndex = draft.Editor.Renderer == "directwrite" ? 1 : 0 };
        renderer.SelectedIndexChanged += () => draft.Editor.Renderer = renderer.SelectedIndex == 1 ? "directwrite" : "gdi";
        yield return new SettingRow("Renderização",
            "GDI é o mais leve. DirectWrite suaviza o texto e junta ligaduras (≥, <>), mas usa ~40 MB a mais.", renderer);
    }

    public override void Reset(AppSettings draft)
    {
        draft.Window.ShowOpenFiles = true;
        draft.Window.ReopenSession = true;
        draft.Editor.Renderer = "gdi";
    }
}
