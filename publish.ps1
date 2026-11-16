<#
.SYNOPSIS
  Publica o Notas como um único .exe (ReadyToRun) em artifacts\publish\<variante>.

.PARAMETER Mode
  fd  = framework-dependent: exe pequeno, precisa do .NET 10 Desktop Runtime instalado.
  sc  = self-contained: leva o runtime junto, roda em qualquer Windows x64.
  all = fd + sc + sc compactado (só para comparar tamanhos).

.EXAMPLE
  .\publish.ps1
  .\publish.ps1 -Mode fd
#>
param(
    [ValidateSet('fd', 'sc', 'all')][string]$Mode = 'all',
    [string]$Runtime = 'win-x64'
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'src\Notas\Notas.csproj'
$common = @(
    '-c', 'Release', '-r', $Runtime, '-nologo',
    '-p:PublishSingleFile=true',
    '-p:PublishReadyToRun=true',
    '-p:DebugType=none'
)

function Publish-Variant([string]$Name, [string[]]$Extra) {
    $out = Join-Path $PSScriptRoot "artifacts\publish\$Name"
    if (Test-Path $out) { Remove-Item $out -Recurse -Force }

    Write-Host "==> $Name" -ForegroundColor Cyan
    dotnet publish $project @common @Extra -o $out | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "Falha no publish ($Name)." }

    $files = @(Get-ChildItem $out -File -Recurse)
    [pscustomobject]@{
        Variante       = $Name
        Arquivos       = $files.Count
        'Tamanho (MB)' = [math]::Round(($files | Measure-Object Length -Sum).Sum / 1MB, 1)
        Exe            = Join-Path $out 'Notas.exe'
    }
}

$results = @()
if ($Mode -in 'fd', 'all') { $results += Publish-Variant 'fd' @('--self-contained', 'false') }
if ($Mode -in 'sc', 'all') { $results += Publish-Variant 'sc' @('--self-contained', 'true') }
if ($Mode -eq 'all') {
    $results += Publish-Variant 'sc-compactado' @('--self-contained', 'true', '-p:EnableCompressionInSingleFile=true')
}

$results | Format-Table -AutoSize
