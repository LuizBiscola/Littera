using Notas.Theme;

namespace Notas.UI.Settings;

internal sealed partial class ShortcutsPage
{
    /// <summary>
    /// Os comandos num controle só, desenhando apenas as linhas visíveis. Com uma linha e uma tecla por comando eram
    /// ~90 janelas: a página levava quase meio segundo para abrir.
    /// </summary>
    private sealed class ShortcutList : Control
    {
        private const int RowHeight = 52;
        private const TextFormatFlags Line = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;

        private readonly (string Command, string Keys)[] _rows;

        public ShortcutList(IEnumerable<KeyValuePair<string, string>> shortcuts)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            _rows = [.. shortcuts.OrderBy(s => s.Key).Select(s => (s.Key, s.Value))];
            BackColor = Palette.Background;
            Height = _rows.Length * this.Px(RowHeight);
        }

        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            Height = _rows.Length * this.Px(RowHeight);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            int height = this.Px(RowHeight);
            using var line = new Pen(Palette.Border);
            for (int i = Math.Max(0, e.ClipRectangle.Top / height); i < _rows.Length && i * height < e.ClipRectangle.Bottom; i++)
            {
                var row = new Rectangle(0, i * height, Width, height);
                TextRenderer.DrawText(g, _rows[i].Command, UiFonts.Get(13, DeviceDpi), row, Palette.Text, Line);
                PaintKeys(g, _rows[i].Keys, row);
                g.DrawLine(line, 0, row.Bottom - 1, Width, row.Bottom - 1);
            }
        }

        /// <summary>Atalho como tecla (fundo cinza claro, cantos redondos); comando sem atalho fica só com o aviso.</summary>
        private void PaintKeys(Graphics g, string keys, Rectangle row)
        {
            Font font = UiFonts.Get(12, DeviceDpi);
            if (keys.Length == 0)
            {
                TextRenderer.DrawText(g, "sem atalho", font, row, Palette.TextFaint, Line | TextFormatFlags.Right);
                return;
            }

            int width = TextRenderer.MeasureText(keys, font).Width + this.Px(16);
            var key = new Rectangle(row.Right - width, row.Y + (row.Height - this.Px(26)) / 2, width, this.Px(26));
            Shapes.FillRounded(g, Palette.ItemHover, key, this.Px(5));
            TextRenderer.DrawText(g, keys, font, key, Palette.TextSecondary, Line | TextFormatFlags.HorizontalCenter);
        }
    }
}
