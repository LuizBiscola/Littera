using Littera.UI.Commands;

namespace Littera.Tests;

public class CommandFilterTests
{
    private static readonly PaletteItem[] Items =
    [
        new("Novo arquivo", "Ctrl+N", null, () => { }),
        new("Recolher trecho", null, null, () => { }),
        new("Recolher tudo", null, null, () => { }),
        new("Configurações", "Ctrl+Comma", null, () => { }),
        new("Linguagem: SQL", null, null, () => { }),
        new("Salvar com encoding: UTF-8", null, null, () => { }),
    ];

    private static IEnumerable<string> Labels(string query) => CommandFilter.Filter(Items, query).Select(i => i.Label);

    [Fact]
    public void Ignora_acentos_e_maiusculas()
    {
        Assert.Equal(["Configurações"], Labels("CONFIGURACOES"));
    }

    [Fact]
    public void Todas_as_palavras_precisam_aparecer()
    {
        Assert.Equal(["Recolher tudo"], Labels("recolh tud"));
    }

    [Fact]
    public void Comeco_do_nome_vem_antes_de_comeco_de_palavra_e_do_meio_da_palavra()
    {
        Assert.Equal(["Configurações", "Salvar com encoding: UTF-8", "Recolher trecho", "Recolher tudo"], Labels("co"));
    }

    [Fact]
    public void Sem_texto_mostra_tudo_na_ordem_original()
    {
        Assert.Equal(Items.Select(i => i.Label), Labels("  "));
    }
}
