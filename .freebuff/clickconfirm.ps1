$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Cf {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 0 }
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$target = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'ConfirmButton') { $target = $e; break } }
if (-not $target) {
    # fallback: search all windows
    $rootEl2 = [System.Windows.Automation.AutomationElement]::RootElement
    $cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
    $wins = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
    foreach ($w in $wins) {
        $all2 = $w.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
        foreach ($e in $all2) { if ($e.Current.AutomationId -eq 'ConfirmButton') { $target = $e; break } }
        if ($target) { break }
    }
}
if (-not $target) { Write-Output "CONFIRM_NOT_FOUND"; exit 1 }
$r = $target.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32Cf]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 300
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 150
[Win32Cf]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 60
[Win32Cf]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 3000
$proc.Refresh()
Write-Output "CLICKED_CONFIRM at=$x,$y aliveAfter=$(-not $proc.HasExited)"
