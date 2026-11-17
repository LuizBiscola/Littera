using Littera.Settings;
using Littera.Theme;
using Littera.UI.Controls;

namespace Littera.UI.Settings;

/// <summary>Uma página do menu lateral das configurações. As linhas editam o rascunho; nada é aplicado antes de "Salvar".</summary>
internal abstract class SettingsPage
{
    public abstract string Name { get; }

    public abstract char Icon { get; }

    public abstract string Subtitle { get; }

    public abstract IEnumerable<Control> Build(AppSettings draft);

    /// <summary>"Restaurar padrões": volta os valores desta página aos de fábrica.</summary>
    public abstract void Reset(AppSettings draft);

    /// <summary>Título de seção em versalete ("TEXTO", "DESTAQUES").</summary>
    protected static Control Section(string title) => new SectionLabel(title);

    /// <summary>Linha com chave liga/desliga ligada a uma propriedade do rascunho.</summary>
    protected static SettingRow Toggle(string label, string? description, bool value, Action<bool> set)
    {
        var toggle = new ToggleSwitch { Checked = value };
        toggle.CheckedChanged += () => set(toggle.Checked);
        return new SettingRow(label, description, toggle);
    }

    private sealed class SectionLabel : Control
    {
        public SectionLabel(string title)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Text = title;
            BackColor = Palette.Background;
            Height = this.Px(40);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            TextRenderer.DrawText(e.Graphics, Text, UiFonts.Semibold(10.5f, DeviceDpi), new Point(0, Height - this.Px(22)),
                Palette.TextFaint, TextFormatFlags.NoPadding);
        }
    }
}
