using Notas.UI.Chrome;

namespace Notas.UI.Controls;

/// <summary>
/// Painel com rolagem que repinta tudo na hora a cada passo. Rolando rápido, o Windows copiava para a posição nova
/// pedaços de linhas (controles filhos) que ainda não tinham sido repintados, e sobravam restos repetidos na tela.
/// </summary>
internal sealed class ScrollPanel : Panel
{
    protected override void OnScroll(ScrollEventArgs se)
    {
        base.OnScroll(se);
        Win32.RedrawNow(Handle);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        Win32.RedrawNow(Handle);
    }
}
