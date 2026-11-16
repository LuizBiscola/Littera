# Medições

Medido com `tools\measure.ps1` (mediana de 8–10 aberturas, pasta de dados limpa via `NOTAS_HOME`).
"Abertura" = criação do processo até a janela pintada e ociosa. "Memória" = coluna do Gerenciador de Tarefas
(working set privado). O **piso** é um WinForms vazio publicado do mesmo jeito, medido na mesma hora:
a máquina tem bastante ruído (até ±150 ms entre execuções), então compare sempre com o piso.

## Fase 0 — base (monitor em 100%)

| | Abertura | Piso | Memória | Exe |
|---|---|---|---|---|
| fd | 400 ms | 306 ms | 7,6 MB | 3,0 MB |
| sc | 331 ms | — | 9,8 MB | 117,8 MB |

## Fase 1 — MVP (monitor em 150%, CPU ~35% ocupada por outros processos)

| | Abertura | Piso | Memória | Exe |
|---|---|---|---|---|
| fd | 480–600 ms | 350–360 ms | 10,9 MB | 3,4 MB |
| sc | 440 ms | 350–360 ms | 12,5 MB | 118,5 MB |

Arquivo de 50 MB (fd): abre em ~2,2 s com o texto aparecendo aos poucos; maior travamento da interface **~56 ms**;
memória com o arquivo aberto ~169 MB.

## Fase 2 — formatador, rascunhos, modo foco, atalhos, configurações (monitor em 150%)

| | Abertura | Piso | Memória | Exe |
|---|---|---|---|---|
| fd | 590 ms (460–630) | 348 ms | 11,1 MB | 3,5 MB |
| sc | 471 ms (353–499) | 348 ms | 12,4 MB | 118,7 MB |

Arquivo de 50 MB (fd): maior travamento **~50 ms**, memória ~167 MB. As novidades da Fase 2 não entram na
abertura (formatador, configurações e busca só carregam no primeiro uso; rascunhos gravam depois de uma pausa).

## Extras — painel "PASTA" e busca em abas/pasta (monitor em 150%)

Medido intercalando execuções (6 de cada), porque o ruído da máquina passa de 100 ms entre rodadas:

| | Abertura (mediana) | Memória |
|---|---|---|
| fd, sem pasta | ~500 ms | 8,7 MB |
| fd, com pasta restaurada (2 subpastas abertas) | ~545 ms | 9,0 MB |
| piso (WinForms vazio, mesma hora) | 378 ms | 6,1 MB |

Sem pasta aberta o painel nem é criado. Com pasta, a árvore usa comparação ordinal: a ordenação por cultura
carregava o ICU na abertura e custava 30–75 ms; agora a árvore leva 6–17 ms. A contagem do rodapé
("N arquivos · M pastas") só começa depois que a janela fica pronta.

O que pesa na abertura (marcas `-ShowMarks`): runtime até o `Main` (~100 ms, igual ao piso), construtor da
janela com barra, painel, status e o primeiro editor (~150 ms), e a primeira pintura (~100 ms).
Já otimizado: DLLs nativas carregadas em segundo plano, configurações e sessão lidas em paralelo,
layout agrupado, arrastar-e-soltar registrado depois da janela pronta, busca e "ir para linha" criadas só no primeiro uso.
