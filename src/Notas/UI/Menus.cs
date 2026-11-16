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

    private static readonly Dictionary<(char, Color, int), Bitmap> Glyphs = [];

    /// <summary>Ícone da Segoe Fluent como imagem de item de menu (guardado: menus são recriados a cada clique).</summary>
    public static Bitmap Glyph(char glyph, Color color, int size, int dpi)
    {
        if (Glyphs.TryGetValue((glyph, color, size), out Bitmap? cached))
            return cached;
        var image = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(image))
        {
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit; // ClearType não funciona em fundo transparente
            using var brush = new SolidBrush(color);
            using var center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(glyph.ToString(), UiFonts.Icon(10.5f, dpi), brush, new RectangleF(0, 0, size, size), center);
        }

        Glyphs[(glyph, color, size)] = image;
        return image;
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
