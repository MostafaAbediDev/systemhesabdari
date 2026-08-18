param([string]$WinName = '', [string]$BtnName = '')
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Btn {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$rootEl = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
$target = $null
for ($i = 0; $i -lt $wins.Count; $i++) {
    $w = $wins.Item($i)
    if ($WinName -and $w.Current.Name -ne $WinName) { continue }
    $all = $w.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
    foreach ($e in $all) {
        if ($e.Current.ControlType -eq [System.Windows.Automation.ControlType]::Button -and $e.Current.Name -eq $BtnName) { $target = $e; break }
    }
    if ($target) { break }
}
if (-not $target) { Write-Output "BTN_NOT_FOUND '$BtnName' in '$WinName'"; exit 1 }
$r = $target.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32Btn]::SetForegroundWindow($proc.MainWindowHandle)
Start-Sleep -Milliseconds 300
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 150
[Win32Btn]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 60
[Win32Btn]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 3000
$proc.Refresh()
Write-Output "BTN_CLICKED '$BtnName' at=$x,$y alive=$(-not $proc.HasExited) responding=$($proc.Responding)"
