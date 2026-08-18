param(
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\closeyesno.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Yn {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$sb = New-Object System.Text.StringBuilder
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$noPane = $null
foreach ($e in $all) { if ($e.Current.Name -eq 'No') { $noPane = $e; break } }
if (-not $noPane) { [void]$sb.AppendLine("NO_PANE_NOT_FOUND"); [IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NOT_FOUND"; exit 1 }
$r = $noPane.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32Yn]::SetForegroundWindow($proc.MainWindowHandle)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 200
[Win32Yn]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32Yn]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
[void]$sb.AppendLine("CLICKED No at=$x,$y")
Start-Sleep -Milliseconds 1500
$proc.Refresh()
$rootEl2 = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all2 = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$still = $false
foreach ($e in $all2) { if ($e.Current.Name -match 'ذخیره تغییرات') { $still = $true } }
[void]$sb.AppendLine("DIALOG_STILL_OPEN=$still")
[void]$sb.AppendLine("alive=$(-not $proc.HasExited) responding=$($proc.Responding)")
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
