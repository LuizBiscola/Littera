using Notas.Theme;

namespace Notas.UI;

/// <summary>Menus suspensos no estilo do protótipo (fundo branco, borda clara, item em destaque cinza).</summary>
internal static class Menus
{
    public static ContextMenuStrip Create(Control owner)
    {
        var menu = new ContextMenuStrip
        {
            Renderer = Renderer(),
            Font = UiFonts.Get(12.5f, owner.DeviceDpi),
            ShowImageMargin = false,
            ShowCheckMargin = true,
        };
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

    /// <summary>Cores do protótipo para menus e listas suspensas.</summary>
    public static ToolStripRenderer Renderer() => new ToolStripProfessionalRenderer(new MenuColors()) { RoundedEdges = false };

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
