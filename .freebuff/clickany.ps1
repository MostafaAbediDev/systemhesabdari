param([string]$Name = 'OK')
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Ok {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$rootEl = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
$target = $null
foreach ($w in $wins) {
    $all = $w.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
    foreach ($e in $all) {
        if ($e.Current.Name -eq $Name) { $target = $e; break }
    }
    if ($target) { break }
}
if (-not $target) { Write-Output "NOT_FOUND name='$Name'"; exit 1 }
$r = $target.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32Ok]::SetForegroundWindow($proc.MainWindowHandle)
Start-Sleep -Milliseconds 300
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 150
[Win32Ok]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 60
[Win32Ok]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 2500
$proc.Refresh()
Write-Output "CLICKED '$Name' type=$($target.Current.ControlType.ProgrammaticName) at=$x,$y alive=$(-not $proc.HasExited) responding=$($proc.Responding)"
