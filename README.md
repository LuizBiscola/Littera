# Notas

Editor de texto minimalista para Windows, focado em SQL e texto. C# / .NET 10 / WinForms + [Scintilla5.NET](https://www.nuget.org/packages/Scintilla5.NET) (Scintilla 5.6.7 + Lexilla 5.5.4).

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
