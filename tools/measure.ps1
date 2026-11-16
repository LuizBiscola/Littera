<#
.SYNOPSIS
  Mede o Notas: tempo de abertura, memória ociosa e (opcional) abertura de arquivo grande.

.DESCRIPTION
  Abre o exe N vezes com NOTAS_METRICS apontando para um arquivo temporário. O app grava:
    ready_ms              tempo desde a criação do processo até a janela pintada e ociosa
    load_ms               tempo para ler o arquivo e exibi-lo
    load_max_ui_stall_ms  maior intervalo em que a UI ficou sem responder durante a leitura
  Depois de IdleSeconds parado, lê Working Set e Private Bytes do processo.

.EXAMPLE
  .\tools\measure.ps1 -Exe .\artifacts\publish\fd\Notas.exe -File .\samples\exemplo.sql
  .\tools\measure.ps1 -Exe .\artifacts\publish\fd\Notas.exe -File .\artifacts\bench\grande-50mb.sql -Runs 3
  .\tools\measure.ps1 -Exe .\artifacts\publish\fd\Notas.exe -Env @{ NOTAS_TECH = 'gdi' }
#>
param(
    [Parameter(Mandatory)][string]$Exe,
    [string]$File,
    [int]$Runs = 10,
    [int]$IdleSeconds = 4,
    [hashtable]$Env = @{},
    [switch]$ShowMarks
)

$ErrorActionPreference = 'Stop'
$Exe = (Resolve-Path $Exe).Path
if ($File) { $File = (Resolve-Path $File).Path }

function Get-Median([double[]]$Values) {
    $sorted = $Values | Sort-Object
    $mid = [int][math]::Floor($sorted.Count / 2)
    if ($sorted.Count % 2) { $sorted[$mid] } else { ($sorted[$mid - 1] + $sorted[$mid]) / 2 }
}

$rows = @()
for ($i = 1; $i -le $Runs; $i++) {
    $metrics = [System.IO.Path]::GetTempFileName()
    $env:NOTAS_METRICS = $metrics
    # Pasta de dados limpa: a medição não depende (nem altera) a sessão real do usuário.
    if (-not $Env.ContainsKey('NOTAS_HOME')) { $env:NOTAS_HOME = Join-Path ([System.IO.Path]::GetTempPath()) "notas-medicao-$i" }
    foreach ($key in $Env.Keys) { Set-Item "env:$key" $Env[$key] }

    try {
        $arguments = if ($File) { "`"$File`"" } else { $null }
        $process = if ($arguments) { Start-Process $Exe -ArgumentList $arguments -PassThru } else { Start-Process $Exe -PassThru }

        $expected = if ($File) { 'load_ms' } else { 'ready_ms' }
        $deadline = (Get-Date).AddSeconds(120)
        while (-not (Select-String -Path $metrics -Pattern "^$expected=" -Quiet)) {
            if ($process.HasExited) { throw "O processo terminou antes de gravar $expected (código $($process.ExitCode))." }
            if ((Get-Date) -gt $deadline) { throw "Tempo esgotado esperando $expected." }
            Start-Sleep -Milliseconds 50
        }

        Start-Sleep -Seconds $IdleSeconds
        $process.Refresh()

        $values = [ordered]@{}
        Get-Content $metrics | ForEach-Object { $k, $v = $_ -split '=', 2; $values[$k] = [double]::Parse($v, [cultureinfo]::InvariantCulture) }

        # "Memória" do Gerenciador de Tarefas = Working Set privado.
        $perf = Get-CimInstance Win32_PerfRawData_PerfProc_Process -Filter "IDProcess = $($process.Id)"

        $rows += [pscustomobject]@{
            Execucao          = $i
            'Abertura (ms)'   = $values['ready_ms']
            'Arquivo (ms)'    = $values['load_ms']
            'Trava UI (ms)'   = $values['load_max_ui_stall_ms']
            'Gerenc. Tarefas (MB)' = [math]::Round($perf.WorkingSetPrivate / 1MB, 1)
            'WorkingSet (MB)' = [math]::Round($process.WorkingSet64 / 1MB, 1)
            'Privada (MB)'    = [math]::Round($process.PrivateMemorySize64 / 1MB, 1)
        }
        if ($ShowMarks) {
            ($values.GetEnumerator() | ForEach-Object { "{0}={1:N0}" -f $_.Key, $_.Value }) -join '  ' | Out-Host
        }

        [void]$process.CloseMainWindow()
        if (-not $process.WaitForExit(5000)) { $process.Kill() }
    }
    finally {
        Remove-Item $metrics -ErrorAction SilentlyContinue
        Remove-Item env:NOTAS_METRICS -ErrorAction SilentlyContinue
        if ($env:NOTAS_HOME -like '*notas-medicao-*') { Remove-Item $env:NOTAS_HOME -Recurse -Force -ErrorAction SilentlyContinue; Remove-Item env:NOTAS_HOME }
        foreach ($key in $Env.Keys) { Remove-Item "env:$key" -ErrorAction SilentlyContinue }
    }
}

$rows | Format-Table -AutoSize | Out-Host

$columns = 'Abertura (ms)', 'Arquivo (ms)', 'Trava UI (ms)', 'Gerenc. Tarefas (MB)', 'WorkingSet (MB)', 'Privada (MB)'
$summary = foreach ($column in $columns) {
    $values = @($rows | ForEach-Object { $_.$column } | Where-Object { $null -ne $_ })
    if ($values.Count -eq 0) { continue }
    [pscustomobject]@{
        Metrica = $column
        Mediana = [math]::Round((Get-Median $values), 1)
        Minimo  = [math]::Round(($values | Measure-Object -Minimum).Minimum, 1)
        Maximo  = [math]::Round(($values | Measure-Object -Maximum).Maximum, 1)
    }
}
$summary | Format-Table -AutoSize | Out-Host
