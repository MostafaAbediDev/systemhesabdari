$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win

# restore window to 1600x900
$hwnd = New-Object System.IntPtr($win.Current.NativeWindowHandle)
Add-Type @"
using System;
using System.Runtime.InteropServices;
public static class WinResize2 {
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_NOACTIVATE = 0x0010;
}
"@
[WinResize2]::SetWindowPos($hwnd, [IntPtr]::Zero, 60, 40, 1600, 900, 0x0004 -bor 0x0010)
Wait-Ms 1200

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-final.txt'
Dump-TreeToFile $win $out 8

$info = Select-String -Path $out -Pattern 'نمایش ' | Select-Object -Last 1
$total = Select-String -Path $out -Pattern 'TotalCountTextBlock' | Select-Object -Last 1
$winRect = $win.Current.BoundingRectangle
Write-Output ("WINDOW_SIZE: " + $winRect.Width + "x" + $winRect.Height)
Write-Output ("INFO: " + $info.Line.Trim())
Write-Output ("TOTAL: " + $total.Line.Trim())

# startup log exceptions check
$log = Join-Path $env:TEMP 'taadol-startup.log'
if (Test-Path $log) {
    $ex = Select-String -Path $log -Pattern 'EXCEPTION|FAILED|!!' 
    if ($ex) { Write-Output ("LOG_EXCEPTIONS: " + $ex.Count) } else { Write-Output "LOG_EXCEPTIONS: 0" }
}
$proc = Get-Process -Id $taadolPid -ErrorAction SilentlyContinue
Write-Output ("APP_FINAL_RESPONDING=" + $proc.Responding + " MEM=" + [math]::Round($proc.WorkingSet64/1MB) + "MB")
Write-Output "DONE"
