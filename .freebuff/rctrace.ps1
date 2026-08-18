$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32RCT {
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
# find the first name cell text
$all = Get-Tree
$cell = $null
foreach ($e in $all) {
    if ($e.Current.ControlType.ProgrammaticName -eq 'ControlType.Text' -and $e.Current.BoundingRectangle.Width -gt 0 -and $e.Current.BoundingRectangle.Y -gt 250 -and $e.Current.BoundingRectangle.Y -lt 400 -and $e.Current.Name -ne '') { $cell = $e; break }
}
if (-not $cell) { [void]$sb.AppendLine("CELL_NOT_FOUND"); [IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_rct.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NOT_FOUND"; exit 1 }
$r = $cell.Current.BoundingRectangle
[void]$sb.AppendLine("CELL name='$($cell.Current.Name)' rect=$r")
$x = [int]($r.X + $r.Width / 2); $y = [int]($r.Y + $r.Height / 2)
[void][Win32RCT]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 300
[Win32RCT]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 100
[Win32RCT]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
[void]$sb.AppendLine("RCLICK at=$x,$y")
Start-Sleep -Milliseconds 2500
# enumerate windows of this process
$rootEl2 = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
[void]$sb.AppendLine("WINCOUNT=$($wins.Count)")
for ($i = 0; $i -lt $wins.Count; $i++) {
    $w = $wins.Item($i)
    [void]$sb.AppendLine("  WIN[$i] title='$($w.Current.Name)' class='$($w.Current.ClassName)'")
    $descs = $w.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
    $menu = 0
    foreach ($d in $descs) {
        if ($d.Current.ControlType.ProgrammaticName -match 'MenuItem|Menu') { $menu++ }
    }
    [void]$sb.AppendLine("    menuElements=$menu")
}
# also search main tree for menu items
$all3 = Get-Tree
$mi = 0
foreach ($e in $all3) { if ($e.Current.ControlType.ProgrammaticName -match 'MenuItem') { $mi++ } }
[void]$sb.AppendLine("MAIN_TREE_MENUITEMS=$mi")
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_rct.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
