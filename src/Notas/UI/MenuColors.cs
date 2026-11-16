using Notas.Theme;

namespace Notas.UI;

/// <summary>Cores que o renderizador base ainda usa (marca de item escolhido e submenus).</summary>
internal sealed class MenuColors : ProfessionalColorTable
{
    public override Color ToolStripDropDownBackground => Palette.Background;
    public override Color MenuBorder => Palette.Border;
    public override Color MenuItemBorder => Palette.AccentSoft;
    public override Color MenuItemSelected => Palette.AccentSoft;
    public override Color ImageMarginGradientBegin => Palette.Background;
    public override Color ImageMarginGradientMiddle => Palette.Background;
    public override Color ImageMarginGradientEnd => Palette.Background;
    public override Color SeparatorDark => Palette.Border;
    public override Color CheckBackground => Palette.AccentSoft;
    public override Color CheckSelectedBackground => Palette.AccentSoft;
    public override Color CheckPressedBackground => Palette.AccentSoft;
}
