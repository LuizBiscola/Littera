namespace Notas.UI;

/// <summary>
/// O WinForms reescreve o Padding do menu a cada layout com <see cref="DefaultPadding"/>, com 2 px em cima e embaixo;
/// aqui vêm os 5 px do protótipo. Dos lados não adianta: o item de menu sempre começa no x 0 (o recuo é do desenho).
/// </summary>
internal sealed class PaddedMenu(int dpi) : ContextMenuStrip
{
    protected override Padding DefaultPadding
    {
        get
        {
            Padding text = base.DefaultPadding;
            int edge = (int)Math.Round(5 * (dpi > 0 ? dpi : 96) / 96.0);
            return new Padding(text.Left, edge, text.Right, edge);
        }
    }
}
