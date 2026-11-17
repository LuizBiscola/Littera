using Notas.Theme;
using Notas.UI.Chrome;

namespace Notas.UI;

/// <summary>Menus suspensos no estilo do protótipo: o desenho fica com <see cref="MenuRenderer"/>.</summary>
internal static partial class Menus
{
    public static ContextMenuStrip Create(Control owner)
    {
        var menu = new PaddedMenu(owner.DeviceDpi) { ShowImageMargin = false, ShowCheckMargin = true };
        Style(menu, owner.DeviceDpi);
        menu.Closed += (_, _) =>
        {
            // Menus são descartáveis (recriados a cada clique); com a janela já fechando, descarta na hora.
            if (owner.IsHandleCreated && !owner.IsDisposed)
                owner.BeginInvoke(menu.Dispose);
            else
                menu.Dispose();
        };
        return menu;
    }

    /// <summary>Submenu com a mesma margem e o mesmo desenho do menu de cima.</summary>
    public static void Submenu(ToolStripMenuItem item, int dpi)
    {
        var menu = new PaddedMenu(dpi) { ShowImageMargin = false };
        Style(menu, dpi);
        item.DropDown = menu;
        item.Disposed += (_, _) => menu.Dispose(); // atribuído à mão: o item não descarta sozinho
    }

    /// <summary>Menu do protótipo: desenho próprio, fonte de 12,5 px, itens de 30 px e cantos arredondados.</summary>
    public static void Style(ToolStripDropDown menu, int dpi)
    {
        int Px(int px) => (int)Math.Round(px * dpi / 96.0);
        menu.Renderer = new MenuRenderer(dpi);
        menu.Font = UiFonts.Get(12.5f, dpi);
        menu.DropShadowEnabled = !Win32.IsWindows11; // no 11 a sombra vem com os cantos arredondados
        menu.HandleCreated += (_, _) => Win32.RoundCorners(menu.Handle, Palette.Border);
        int extra = Math.Max(0, Px(30) - menu.Font.Height - Px(2)); // altura do texto + 1 px de cada lado no layout
        menu.ItemAdded += (_, e) =>
        {
            if (e.Item is ToolStripMenuItem item)
                item.Padding = new Padding(0, extra / 2, 0, extra - extra / 2);
        };
    }

    /// <summary>Desenho dos menus, para listas suspensas próprias (caixa de fonte).</summary>
    public static ToolStripRenderer Renderer(int dpi) => new MenuRenderer(dpi);

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
}
