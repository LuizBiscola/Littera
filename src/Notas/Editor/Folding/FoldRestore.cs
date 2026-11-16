namespace Notas.Editor.Folding;

/// <summary>
/// Cabeçalhos recolhidos da sessão anterior e a rolagem de então: a primeira linha visível conta
/// linhas na tela, então só volta ao lugar certo depois de recolher os mesmos trechos.
/// </summary>
internal sealed record FoldRestore(IReadOnlyList<int> Headers, int FirstVisibleLine);
