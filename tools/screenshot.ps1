<#
.SYNOPSIS
  Captura a janela principal de um processo em PNG (ferramenta de desenvolvimento).
.EXAMPLE
  .\tools\screenshot.ps1 -ProcessId 1234 -Out .\artifacts\shot.png
#>
param(
    [Parameter(Mandatory)][int]$ProcessId,
    [Parameter(Mandatory)][string]$Out
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class NotasShot {
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left, Top, Right, Bottom; }
    [DllImport("user32.dll")] public static extern IntPtr SetThreadDpiAwarenessContext(IntPtr ctx);
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdc, uint flags);
    [DllImport("dwmapi.dll")] public static extern int DwmGetWindowAttribute(IntPtr hwnd, int attr, out RECT rect, int size);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hwnd, out RECT rect);
}
'@

[void][NotasShot]::SetThreadDpiAwarenessContext([IntPtr]::new(-4)) # Per-monitor v2: pixels reais

$process = Get-Process -Id $ProcessId
$hwnd = $process.MainWindowHandle
if ($hwnd -eq [IntPtr]::Zero) { throw "Processo $ProcessId não tem janela principal." }

$window = New-Object NotasShot+RECT
[void][NotasShot]::GetWindowRect($hwnd, [ref]$window)
$visible = New-Object NotasShot+RECT
[void][NotasShot]::DwmGetWindowAttribute($hwnd, 9, [ref]$visible, 16) # DWMWA_EXTENDED_FRAME_BOUNDS

$width = $window.Right - $window.Left
$height = $window.Bottom - $window.Top
$full = New-Object System.Drawing.Bitmap $width, $height
$graphics = [System.Drawing.Graphics]::FromImage($full)
$hdc = $graphics.GetHdc()
[void][NotasShot]::PrintWindow($hwnd, $hdc, 2) # PW_RENDERFULLCONTENT
$graphics.ReleaseHdc($hdc)
$graphics.Dispose()

# Recorta a borda invisível (sombra) que o Windows 10/11 inclui no retângulo da janela.
$crop = New-Object System.Drawing.Rectangle ($visible.Left - $window.Left), ($visible.Top - $window.Top), ($visible.Right - $visible.Left), ($visible.Bottom - $visible.Top)
$image = $full.Clone($crop, $full.PixelFormat)
New-Item -ItemType Directory -Force (Split-Path $Out) | Out-Null
$image.Save($Out, [System.Drawing.Imaging.ImageFormat]::Png)
$image.Dispose(); $full.Dispose()
Write-Output $Out
