param(
    [string]$Cell = 'اریاگستر',
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\verify_rcdump.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32RcD {
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
if (-not $found) { [void]$sb.AppendLine("CELL_NOT_FOUND '$Cell'"); [IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NOT_FOUND"; exit 1 }
$r = $found.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32RcD]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 200
[Win32RcD]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32RcD]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
[void]$sb.AppendLine("RCLICK '$Cell' at=$x,$y")
Start-Sleep -Milliseconds 1500
$rootEl2 = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all2 = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
[void]$sb.AppendLine("ELEMENTS=$($all2.Count)")
for ($i = 0; $i -lt $all2.Count; $i++) {
    $e = $all2.Item($i)
    $t = $e.Current.ControlType.ProgrammaticName
    $n = $e.Current.Name
    $id = $e.Current.AutomationId
    $bb = $e.Current.BoundingRectangle
    if ($n -or $id) {
        [void]$sb.AppendLine("[$i] $t | name='$n' | id='$id' | rect=$([int]$bb.X),$([int]$bb.Y) ${($bb.Width)}x${($bb.Height)} | off=$($e.Current.IsOffscreen)")
    }
}
$proc.Refresh()
[void]$sb.AppendLine("alive=$(-not $proc.HasExited) responding=$($proc.Responding)")
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
