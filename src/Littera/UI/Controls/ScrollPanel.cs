namespace Notas.UI.Controls;

/// <summary>
/// Painel com rolagem pintado de uma vez (WS_EX_COMPOSITED): o Windows desenha o painel e todas as linhas filhas fora
/// da tela e mostra o resultado pronto. Sem isso, rolando rápido, aparecia na tela a cópia de pixels que o WinForms faz
/// ao rolar (ScrollWindowEx) antes de cada linha se repintar: faixas velhas repetidas e chaves fora do lugar.
/// </summary>
internal sealed class ScrollPanel : Panel
{
    private const int WS_EX_COMPOSITED = 0x02000000;

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= WS_EX_COMPOSITED;
            return cp;
        }
    }
}
