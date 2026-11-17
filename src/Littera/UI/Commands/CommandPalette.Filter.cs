using Notas.Theme;

namespace Notas.UI.Commands;

internal sealed partial class CommandPalette
{
    /// <summary>Refaz a lista a cada letra; a altura acompanha a quantidade de resultados (até 10 linhas).</summary>
    private void Refilter()
    {
        _shown = CommandFilter.Filter(_items, _query.Text);
        _selected = _top = 0;
        int rows = Math.Clamp(_shown.Count, 1, MaxRows);
        Size = new Size(this.Px(PaletteWidth), this.Px(InputHeight + 6) + rows * this.Px(RowHeight));
        if (Parent is not null)
            Location = new Point(Math.Max(0, (Parent.ClientSize.Width - Width) / 2), this.Px(10));
        _query.Font = UiFonts.Get(13, DeviceDpi);
        _query.SetBounds(this.Px(20), (this.Px(InputHeight) - _query.PreferredHeight) / 2, Width - this.Px(40), _query.PreferredHeight);
        Invalidate();
    }
}
