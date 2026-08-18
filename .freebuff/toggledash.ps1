param(
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\toggledash.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Td {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$btn = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'BtnDashboard') { $btn = $e; break } }
if (-not $btn) { Write-Output "BTN_NOT_FOUND"; exit 1 }
$r = $btn.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32Td]::SetForegroundWindow($proc.MainWindowHandle)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 200
[Win32Td]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32Td]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 1500
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("CLICK BtnDashboard at=$x,$y")
$rootEl2 = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all2 = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
for ($i=0; $i -lt $all2.Count; $i++) {
    $e = $all2.Item($i)
    if ($e.Current.Name -match 'نمای کلی|داشبورد|گزارش|لیست اشخاص') {
        [void]$sb.AppendLine("[$i] name='$($e.Current.Name)' id='$($e.Current.AutomationId)' off=$($e.Current.IsOffscreen)")
    }
}
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
