using ScintillaNET;

namespace Notas.UI.Find;

/// <summary>Arquivo de um escopo com vários arquivos. Key é o caminho, ou "*N" para a aba "Sem título N".</summary>
internal sealed record FindFile(string Key, string? Path);

/// <summary>O que a janela oferece à busca nos escopos "Abas abertas" e "Pasta".</summary>
internal interface IFindFiles
{
    bool HasFolder { get; }

    FindFile? Active { get; }

    /// <summary>O arquivo ativo faz parte do escopo (na "Pasta", só se estiver dentro dela)?</summary>
    bool ActiveInScope(SearchScope scope);

    /// <summary>Arquivos do escopo na ordem de navegação (a pasta é percorrida em segundo plano).</summary>
    Task<List<FindFile>> ListAsync(SearchScope scope);

    /// <summary>Editor já carregado do arquivo (aba aberta); null = ler do disco.</summary>
    Scintilla? LoadedEditor(FindFile file);

    /// <summary>Ativa a aba, abrindo o arquivo se preciso, e espera terminar de carregar.</summary>
    Task<Scintilla?> ShowAsync(FindFile file);
}
