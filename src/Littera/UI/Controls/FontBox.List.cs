using Littera.Theme;

namespace Littera.UI.Controls;

internal sealed partial class FontBox
{
    /// <summary>Lista suspensa de fontes instaladas, com a atual marcada; clique ou Enter escolhe.</summary>
    private void ShowList()
    {
        var list = new ListBox
        {
            BorderStyle = BorderStyle.None,
            DrawMode = DrawMode.OwnerDrawFixed,
            IntegralHeight = false,
            ItemHeight = this.Px(28),
            Size = new Size(Width - 2, this.Px(280)),
        };
        list.Items.AddRange(_fonts);
        list.SelectedItem = _fonts.FirstOrDefault(f => string.Equals(f, Value, StringComparison.OrdinalIgnoreCase));
        list.DrawItem += DrawFont;

        // ToolStripDropDown simples (não menu): sem margens de ícone, fica da largura da caixa.
        var menu = new ToolStripDropDown { Renderer = Menus.Renderer(DeviceDpi), Padding = new Padding(1) };
        menu.HandleCreated += (_, _) => Chrome.Win32.RoundCorners(menu.Handle, Palette.Border);
        menu.Items.Add(new ToolStripControlHost(list) { AutoSize = false, Size = list.Size, Margin = Padding.Empty, Padding = Padding.Empty });
        menu.Closed += (_, _) => BeginInvoke(menu.Dispose);
        void Pick()
        {
            if (list.SelectedItem is string font)
                Value = font;
            menu.Close();
            _text.Focus();
        }

        list.MouseClick += (_, _) => Pick();
        list.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
                Pick();
        };
        menu.Show(this, new Point(0, Height + this.Px(2)));
        list.Focus();
    }

    private void DrawFont(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0 || sender is not ListBox list)
            return;
        bool selected = e.State.HasFlag(DrawItemState.Selected);
        using (var background = new SolidBrush(selected ? Palette.ItemActive : Palette.Background))
            e.Graphics.FillRectangle(background, e.Bounds);
        var text = Rectangle.FromLTRB(e.Bounds.X + this.Px(10), e.Bounds.Y, e.Bounds.Right, e.Bounds.Bottom);
        TextRenderer.DrawText(e.Graphics, (string)list.Items[e.Index], UiFonts.Get(12.5f, DeviceDpi), text, Palette.Text,
            TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
    }
}
