using Notas.Theme;
using Notas.UI.Chrome;

namespace Notas.UI;

/// <summary>Menus suspensos no estilo do protótipo: o desenho fica com <see cref="MenuRenderer"/>.</summary>
internal static partial class Menus
{
    public static ContextMenuStrip Create(Control owner)
    {
        var menu = new ContextMenuStrip { ShowImageMargin = false, ShowCheckMargin = true };
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

    /// <summary>Menu ou submenu: desenho próprio, fonte de 13 px, itens de ~30 px e cantos arredondados.</summary>
    public static void Style(ToolStripDropDown menu, int dpi)
    {
        int Px(int px) => (int)Math.Round(px * dpi / 96.0);
        menu.Renderer = new MenuRenderer(dpi);
        menu.Font = UiFonts.Get(13, dpi);
        menu.Padding = new Padding(Px(4), Px(5), Px(4), Px(5));
        menu.DropShadowEnabled = !Win32.IsWindows11; // no 11 a sombra vem com os cantos arredondados
        menu.HandleCreated += (_, _) => Win32.RoundCorners(menu.Handle, Palette.Border);
        menu.ItemAdded += (_, e) =>
        {
            if (e.Item is ToolStripMenuItem item)
                item.Padding = new Padding(0, Px(4), 0, Px(4));
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
