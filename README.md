<p align="center">
  <img src="docs/images/logo.png" width="88" alt="">
</p>

<h1 align="center">Littera</h1>

<p align="center">
  Editor de texto leve para Windows, feito para SQL e texto.<br>
  C# · .NET 10 · WinForms · Scintilla
</p>

![Littera com uma pasta aberta e uma consulta SQL em edição](docs/images/littera.png)

O nome vem do latim *littera*, "letra". A ideia é a leveza do Notepad++ com um visual limpo: abre rápido, gasta pouca
memória, aguenta arquivos grandes e entende SQL de verdade (blocos, formatação, trechos recolhíveis com resumo).

## Recursos

**Abas e pastas**

- Abas verticais em duas seções, "Fixadas" e "Abertos". A aba fixada não fecha por Ctrl+W nem pelo botão do meio e
  sempre volta ao reabrir. Arraste para reordenar ou mudar de seção.
- Clique direito na aba: fechar outras ou as já salvas, copiar caminho, mostrar na pasta, renomear (F2), duplicar e
  descartar alterações. Arquivo apagado ou movido fora do Littera aparece riscado.
- Painel "Pasta" com árvore, filtro, atualização automática e marca dos arquivos já abertos; Ctrl+P abre um arquivo
  da pasta pelo nome.
- Os dois painéis laterais mudam de largura arrastando a borda (clique duplo volta ao padrão).
- Ícones por tipo de arquivo (SQL, código, JSON, XML/YAML, tabela, configuração, documentação, log, texto).

**Edição**

- Realce de SQL (T-SQL/ANSI), JSON, XML, C#, JavaScript, Python, Markdown e texto; a linguagem também pode ser
  escolhida na barra de status.
- Bloco atual destacado (a instrução SQL ou o parágrafo), ocorrências da palavra do cursor e o parêntese que fecha
  o do cursor (em vermelho quando não tem par).
- Trechos recolhíveis pela seta da calha: instruções, parênteses, BEGIN/CASE…END e comentários em SQL; parágrafos em
  texto; chaves, tags e recuo nas outras linguagens. O trecho recolhido mostra um resumo
  ("8 linhas │ SELECT 4 colunas · FROM pedidos · WHERE 2 filtros"); parar o mouse nele abre a prévia.
- Linhas alteradas marcadas na calha: laranja = não salva, verde = salva. Clicar na barra mostra o que mudou, com
  "Copiar original" e "Reverter".
- Formatador de SQL configurável, comentar/descomentar (`--` em SQL), duplicar e mover linhas, múltiplos cursores,
  seleção em coluna (Alt+arrastar), Enter mantendo a indentação, guias de indentação, zoom e quebra de linha.
- Buscar e substituir com regex, maiúsculas e palavra inteira, no arquivo, no bloco, na seleção, nas abas abertas ou
  na pasta inteira.
- Paleta de comandos (Ctrl+Shift+P) com todos os comandos, linguagem, encoding, quebra de linha e arquivos recentes.

**Arquivos**

- Encoding (UTF-8, UTF-8 com BOM, ANSI, UTF-16) e fim de linha detectados e convertidos pela barra de status.
- Arquivos de 50 MB abrem em blocos, sem travar a janela.
- A sessão volta ao reabrir, com as abas, a posição do cursor e os trechos recolhidos. Alterações não salvas ficam em
  rascunhos e também voltam.
- Arquivo novo é salvo como `.txt` por padrão.

## Atalhos

| Atalho | Comando | Atalho | Comando |
|---|---|---|---|
| Ctrl+N / Ctrl+O | Novo / abrir | Ctrl+F / Ctrl+H | Buscar / substituir |
| Ctrl+Shift+O | Abrir pasta | F3 / Shift+F3 | Próxima / anterior |
| Ctrl+S / Ctrl+Shift+S | Salvar / salvar como | Ctrl+G | Ir para linha |
| Ctrl+W | Fechar aba | Ctrl+Shift+F | Formatar SQL |
| Ctrl+Tab | Próxima aba | F11 | Modo foco |
| Ctrl+B / Ctrl+Shift+E | Painel de abas / de pasta | Alt+Z | Quebra de linha |
| Ctrl+Plus / Minus / 0 | Zoom | Ctrl+Shift+L | Números de linha |
| Ctrl+P | Ir para arquivo da pasta | Ctrl+, | Configurações |
| Ctrl+Shift+[ | Recolher trecho do cursor | Ctrl+Shift+] | Expandir trecho do cursor |
| Ctrl+Shift+P | Paleta de comandos | Ctrl+/ | Comentar ou descomentar |
| Ctrl+D | Duplicar linha ou seleção | Alt+↑ / Alt+↓ | Mover linha |
| Alt+F5 | Próxima alteração | Shift+Alt+F5 | Alteração anterior |
| Ctrl+Alt+P | Fixar ou desafixar a aba | Ctrl+Alt+C | Copiar caminho do arquivo |
| F2 | Renomear o arquivo da aba | Alt | Menu do app |

Os atalhos podem ser trocados no `settings.json` (seção `shortcuts`, vários separados por `;`). Teclas como `[` e `]`
seguem o layout do teclado (ABNT2 ou americano).

## Instalação

O Littera é um único arquivo, `Littera.exe`, sem instalador. Requer Windows 10 ou 11 de 64 bits.

```powershell
.\publish.ps1 -Mode sc   # artifacts\publish\sc\Littera.exe: leva o .NET junto, roda em qualquer Windows x64
.\publish.ps1 -Mode fd   # artifacts\publish\fd\Littera.exe: ~4 MB, precisa do .NET 10 Desktop Runtime
```

Copie o exe para uma pasta fixa (por exemplo `C:\Programas\Littera`) e crie um atalho ou fixe na barra de tarefas.
Como o exe não tem assinatura digital, na primeira vez o Windows pode avisar "O Windows protegeu o computador":
"Mais informações" → "Executar assim mesmo".

## Configurações e dados

| O quê | Onde |
|---|---|
| Configurações (também pela janela Ctrl+,) | `%APPDATA%\Littera\settings.json` |
| Sessão e rascunhos não salvos | `%APPDATA%\Littera\` |
| Log de erros inesperados | `%LOCALAPPDATA%\Littera\erros.log` |
| Scintilla.dll e Lexilla.dll extraídas do exe | `%LOCALAPPDATA%\Littera\native\` |

Para levar suas preferências para outro PC, copie o `settings.json`. O app se chamava Notas: na primeira abertura
como Littera, as pastas `%APPDATA%\Notas` e `%LOCALAPPDATA%\Notas` são renomeadas para `Littera`.

## Créditos

- [Scintilla e Lexilla](https://www.scintilla.org): o componente de edição e os realces de sintaxe.
- [Scintilla5.NET](https://github.com/desjarlais/Scintilla.NET): o controle WinForms sobre o Scintilla.
- [Lucide](https://lucide.dev): os ícones de arquivo e pasta.
- [Fraunces](https://github.com/undercasetype/Fraunces): a fonte do nome na barra de título.

Os avisos de licença desses componentes estão em [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt).
