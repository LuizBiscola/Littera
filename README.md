# Notas

Editor de texto minimalista para Windows, focado em SQL e texto. C# / .NET 10 / WinForms + [Scintilla5.NET](https://www.nuget.org/packages/Scintilla5.NET) (Scintilla 5.6.7 + Lexilla 5.5.4).

## Recursos

- Abas verticais ("ABERTOS") e painel "PASTA" com árvore, filtro e marca dos arquivos já abertos.
- Realce de SQL (T-SQL/ANSI), JSON, XML, C#, JavaScript, Python, Markdown e texto; escolha manual na barra de status.
- Bloco atual (instrução SQL ou parágrafo) destacado e ocorrências da palavra do cursor.
- Buscar e substituir com regex, maiúsculas e palavra inteira, em arquivo, bloco, seleção, abas abertas ou pasta.
- Formatador de SQL configurável, múltiplos cursores, seleção em coluna (Alt+arrastar), zoom, quebra de linha.
- Encoding (UTF-8, UTF-8 com BOM, ANSI) e fim de linha detectados e convertidos pela barra de status.
- Arquivos de 50 MB abrem em blocos sem travar a janela; sessão e rascunhos não salvos voltam ao reabrir.
- Configurações em `%APPDATA%\Notas\settings.json` (também pela janela de configurações, Ctrl+,).

| Atalho | Comando | Atalho | Comando |
|---|---|---|---|
| Ctrl+N / Ctrl+O | Novo / abrir | Ctrl+F / Ctrl+H | Buscar / substituir |
| Ctrl+Shift+O | Abrir pasta | F3 / Shift+F3 | Próxima / anterior |
| Ctrl+S / Ctrl+Shift+S | Salvar / salvar como | Ctrl+G | Ir para linha |
| Ctrl+W | Fechar aba | Ctrl+Shift+F | Formatar SQL |
| Ctrl+Tab | Próxima aba | F11 | Modo foco |
| Ctrl+B / Ctrl+Shift+E | Painel de abas / de pasta | Alt+Z | Quebra de linha |
| Ctrl+Plus / Minus / 0 | Zoom | Ctrl+Shift+L | Números de linha |

Os atalhos podem ser trocados no `settings.json` (seção `shortcuts`).

## Desenvolvimento

```powershell
dotnet build
dotnet run --project src\Notas -- samples\exemplo.sql
```

## Publicação

```powershell
.\publish.ps1            # fd + sc + sc-compactado em artifacts\publish\
.\publish.ps1 -Mode fd   # só framework-dependent (exige .NET 10 Desktop Runtime)
```

O exe é single-file: `Scintilla.dll` e `Lexilla.dll` vão embutidas e são extraídas na primeira execução para
`%LOCALAPPDATA%\Notas\native\<versão>\` (a cópia em disco é conferida byte a byte com a embutida a cada abertura).

## Medição

```powershell
.\tools\measure.ps1 -Exe .\artifacts\publish\fd\Notas.exe -File .\samples\exemplo.sql
.\tools\make-bigfile.ps1    # gera artifacts\bench\grande-50mb.sql
.\tools\measure.ps1 -Exe .\artifacts\publish\fd\Notas.exe -File .\artifacts\bench\grande-50mb.sql -Runs 3
```

`measure.ps1` reporta abertura (criação do processo até a janela pintada e ociosa), tempo do arquivo,
maior travamento da UI e memória (coluna "Gerenc. Tarefas" = o que o Gerenciador de Tarefas mostra).

Medições de cada fase: [docs/medicoes.md](docs/medicoes.md).

`NOTAS_HOME` troca a pasta de configurações e sessão (o `measure.ps1` usa uma pasta limpa a cada execução).
