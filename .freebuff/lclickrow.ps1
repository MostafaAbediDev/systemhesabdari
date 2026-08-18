param(
    [string]$Cell = 'اریاگستر',
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\verify_lclick.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32LcR {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$sb = New-Object System.Text.StringBuilder
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$found = $null
foreach ($e in $all) { if ($e.Current.Name -eq $Cell -and $e.Current.ControlType.ProgrammaticName -match 'Text') { $found = $e; break } }
if (-not $found) { Write-Output "CELL_NOT_FOUND"; exit 1 }
$r = $found.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32LcR]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 200
[Win32LcR]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 60
[Win32LcR]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
[void]$sb.AppendLine("LCLICK '$Cell' at=$x,$y")
Start-Sleep -Milliseconds 2000
$rootEl2 = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all2 = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
foreach ($e in $all2) {
    if ($e.Current.AutomationId -eq 'SelectedCountText' -or $e.Current.AutomationId -eq 'PanelHeaderText' -or $e.Current.AutomationId -eq 'SelectedSummaryText') {
        [void]$sb.AppendLine("  id='$($e.Current.AutomationId)' name='$($e.Current.Name)'")
    }
}
$proc.Refresh()
[void]$sb.AppendLine("alive=$(-not $proc.HasExited) responding=$($proc.Responding)")
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
