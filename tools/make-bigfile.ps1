<#
.SYNOPSIS
  Gera um arquivo SQL grande (padrão 50 MB, UTF-8 com acentos) para testar a abertura sem travar.
.EXAMPLE
  .\tools\make-bigfile.ps1
  .\tools\make-bigfile.ps1 -SizeMB 100 -Out .\artifacts\bench\grande-100mb.sql
#>
param(
    [int]$SizeMB = 50,
    [string]$Out = (Join-Path $PSScriptRoot "..\artifacts\bench\grande-$($SizeMB)mb.sql")
)

$ErrorActionPreference = 'Stop'
$template = (Get-Content (Join-Path $PSScriptRoot '..\samples\exemplo.sql') -Raw -Encoding utf8) -replace "`r?`n", "`r`n"
New-Item -ItemType Directory -Force (Split-Path $Out) | Out-Null

$target = [long]$SizeMB * 1MB
$writer = [System.IO.StreamWriter]::new($Out, $false, [System.Text.UTF8Encoding]::new($false))
try {
    $block = 0
    while ($writer.BaseStream.Length -lt $target) {
        $block++
        $writer.Write("-- bloco $block`r`n")
        $writer.Write($template)
        $writer.Flush()
    }
}
finally { $writer.Dispose() }

$info = Get-Item $Out
"{0} ({1:N1} MB, {2:N0} blocos)" -f $info.FullName, ($info.Length / 1MB), $block
