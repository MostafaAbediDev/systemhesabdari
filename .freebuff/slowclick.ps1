param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Slow {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
function Get-All { $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition) }

# find the Text 'مشتری' inside tabCustomers
$all = Get-All
$text = $null
foreach ($e in $all) { if ($e.Current.Name -eq 'مشتری' -and $e.Current.ControlType.ProgrammaticName -eq 'ControlType.Text') { $text = $e; break } }
if (-not $text) { Write-Output "TEXT_NOT_FOUND"; exit 1 }
$r = $text.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
Write-Output "Text 'مشتری' rect=$($r.X),$($r.Y),$($r.Width)x$($r.Height) center=$x,$y"
[void][Win32Slow]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 600
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 600
[Win32Slow]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 250
[Win32Slow]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 3000

# read tab states
$sb = New-Object System.Text.StringBuilder
$all = Get-All
foreach ($e in $all) {
    if ($e.Current.AutomationId -match '^tab') {
        $state = 'N/A'
        try { $tp = $e.GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern); $state = $tp.Current.ToggleState.ToString() } catch {}
        [void]$sb.AppendLine("TAB id=$($e.Current.AutomationId) toggle=$state")
    }
    if ($e.Current.AutomationId -eq 'PageInfoText') { [void]$sb.AppendLine("PAGEINFO=$($e.Current.Name)") }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\sm2_slowtab.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
