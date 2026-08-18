param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32TabT {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
function Get-All { $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition) }

# 1) restore: toggle tabCustomers back to Off (fix my artifact)
$all = Get-All
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'tabCustomers') {
    $tp = $e.GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern)
    if ($tp.Current.ToggleState -eq [System.Windows.Automation.ToggleState]::On) { $tp.Toggle(); Write-Output "RESTORED tabCustomers -> Off" }
    else { Write-Output "tabCustomers already Off" }
} }
Start-Sleep -Milliseconds 1500

# 2) real mouse click on tabCustomers center
$all = Get-All
$tab = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'tabCustomers') { $tab = $e; break } }
if (-not $tab) { Write-Output "TAB_NOT_FOUND"; exit 1 }
$r = $tab.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32TabT]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 500
# move cursor away first, then to target
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point(($x+300), ($y+100))
Start-Sleep -Milliseconds 300
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 300
[Win32TabT]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32TabT]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
Write-Output "REAL_CLICK tabCustomers at=$x,$y"
Start-Sleep -Milliseconds 2500

# 3) read state
$all = Get-All
$sb = New-Object System.Text.StringBuilder
foreach ($e in $all) {
    if ($e.Current.AutomationId -match '^tab') {
        $state = 'N/A'
        try { $tp = $e.GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern); $state = $tp.Current.ToggleState.ToString() } catch {}
        [void]$sb.AppendLine("TAB id=$($e.Current.AutomationId) toggle=$state")
    }
    if ($e.Current.AutomationId -eq 'PageInfoText') { [void]$sb.AppendLine("PAGEINFO=$($e.Current.Name)") }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\sm2_tabresult.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
