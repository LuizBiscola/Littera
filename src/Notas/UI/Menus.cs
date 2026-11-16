using Notas.Theme;

namespace Notas.UI;

/// <summary>Menus suspensos no estilo do protótipo (fundo branco, borda clara, item em destaque cinza).</summary>
internal static class Menus
{
    public static ContextMenuStrip Create(Control owner)
    {
        var menu = new ContextMenuStrip
        {
            Renderer = new ToolStripProfessionalRenderer(new MenuColors()) { RoundedEdges = false },
            Font = UiFonts.Get(12.5f, owner.DeviceDpi),
            ShowImageMargin = false,
            ShowCheckMargin = true,
        };
        menu.Closed += (_, _) => owner.BeginInvoke(menu.Dispose); // menus são descartáveis: recriados a cada clique
        return menu;
    }

    /// <summary>Lista de opções com a atual marcada.</summary>
    public static ContextMenuStrip Choices<T>(Control owner, string? title, IEnumerable<T> values, T current, Func<T, string> label, Action<T> pick)
    {
        ContextMenuStrip menu = Create(owner);
        if (title is not null)
        {
            menu.Items.Add(new ToolStripLabel(title) { ForeColor = Palette.TextFaint });
            menu.Items.Add(new ToolStripSeparator());
        }

        foreach (T value in values)
        {
            T item = value;
            menu.Items.Add(new ToolStripMenuItem(label(item), null, (_, _) => pick(item))
            {
                Checked = EqualityComparer<T>.Default.Equals(item, current),
            });
        }

        return menu;
    }

    private sealed class MenuColors : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => Palette.Background;
        public override Color MenuBorder => Palette.Border;
        public override Color MenuItemBorder => Palette.ItemActive;
        public override Color MenuItemSelected => Palette.ItemActive;
        public override Color ImageMarginGradientBegin => Palette.Background;
        public override Color ImageMarginGradientMiddle => Palette.Background;
        public override Color ImageMarginGradientEnd => Palette.Background;
        public override Color SeparatorDark => Palette.Border;
        public override Color CheckBackground => Palette.AccentSoft;
        public override Color CheckSelectedBackground => Palette.AccentSoft;
    }
}
