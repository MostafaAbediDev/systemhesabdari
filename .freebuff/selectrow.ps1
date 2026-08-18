$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32SelRow {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
function Get-Tree {
    $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
    return $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
}
$sb = New-Object System.Text.StringBuilder
$all = Get-Tree
# find RowToggle buttons with y in first row band
$toggle = $null
foreach ($e in $all) {
    $r = $e.Current.BoundingRectangle
    if ($e.Current.AutomationId -eq 'RowToggle' -and $r.Width -gt 0 -and $r.Y -gt 280 -and $r.Y -lt 330) { $toggle = $e; break }
}
if (-not $toggle) {
    # fallback: any RowToggle
    foreach ($e in $all) { if ($e.Current.AutomationId -eq 'RowToggle') { $toggle = $e; break } }
}
if (-not $toggle) { [void]$sb.AppendLine("TOGGLE_NOT_FOUND"); [IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_selrow.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NOT_FOUND"; exit 1 }
$r = $toggle.Current.BoundingRectangle
[void]$sb.AppendLine("TOGGLE rect=$r")
$x = [int]($r.X + $r.Width / 2); $y = [int]($r.Y + $r.Height / 2)
[void][Win32SelRow]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 300
[Win32SelRow]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32SelRow]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
[void]$sb.AppendLine("CLICK at=$x,$y")
Start-Sleep -Milliseconds 3000
$proc.Refresh()
# count selected + panel presence
$all2 = Get-Tree
$sel = 0; $panel = 0
foreach ($e in $all2) {
    if ($e.Current.AutomationId -eq 'SelectedCountText') { $sel++ }
    if ($e.Current.AutomationId -eq 'PersonNameText') { $panel++ }
    if ($e.Current.AutomationId -eq 'PanelHeaderText') { $panel++ }
}
[void]$sb.AppendLine("RESULT SelectedCountText=$sel Panel=$panel alive=$(-not $proc.HasExited)")
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_selrow.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
