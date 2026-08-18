param(
    [string]$Cell = 'اریاگستر',
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\verify_del_menu2.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32CtxD {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$sb = New-Object System.Text.StringBuilder
$hwnd = $proc.MainWindowHandle

function Get-Tree {
    $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
    return $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
}

$all = Get-Tree
$found = $null
foreach ($e in $all) { if ($e.Current.Name -eq $Cell -and $e.Current.ControlType.ProgrammaticName -match 'Text') { $found = $e; break } }
if (-not $found) { [void]$sb.AppendLine("CELL_NOT_FOUND '$Cell'"); [IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NOT_FOUND"; exit 1 }
$r = $found.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32CtxD]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 200
[Win32CtxD]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32CtxD]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
[void]$sb.AppendLine("RCLICK '$Cell' at=$x,$y")
Start-Sleep -Milliseconds 2500

# find 'حذف' text items; skip the toolbar one (AutomationId='BtnText' or first in tree)
$all2 = Get-Tree
$matches = @()
foreach ($e in $all2) {
    if ($e.Current.Name -eq 'حذف' -and $e.Current.BoundingRectangle.Width -gt 0) {
        $matches += ,@($e.Current.AutomationId, $e.Current.ControlType.ProgrammaticName, $e.Current.BoundingRectangle.X, $e.Current.BoundingRectangle.Y)
    }
}
[void]$sb.AppendLine("DELETE_MATCHES=$($matches.Count)")
foreach ($m in $matches) { [void]$sb.AppendLine("  match id='$($m[0])' type=$($m[1]) at=$($m[2]),$($m[3])") }

$menuItem = $null
foreach ($e in $all2) {
    if ($e.Current.Name -eq 'حذف' -and $e.Current.BoundingRectangle.Width -gt 0 -and $e.Current.AutomationId -ne 'BtnText') { $menuItem = $e; break }
}
if (-not $menuItem) {
    [void]$sb.AppendLine("MENU_ITEM_NOT_FOUND")
} else {
    $mr = $menuItem.Current.BoundingRectangle
    $mx = [int]($mr.X + $mr.Width/2); $my = [int]($mr.Y + $mr.Height/2)
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($mx, $my)
    Start-Sleep -Milliseconds 200
    [Win32CtxD]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 60
    [Win32CtxD]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
    [void]$sb.AppendLine("CLICKED_MENU_DELETE at=$mx,$my")
}
Start-Sleep -Milliseconds 2500
$proc.Refresh()
[void]$sb.AppendLine("after: alive=$(-not $proc.HasExited) responding=$($proc.Responding) memMB=$([math]::Round($proc.WorkingSet64/1MB))")
$rootEl = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
[void]$sb.AppendLine("WINCOUNT=$($wins.Count)")
for ($i = 0; $i -lt $wins.Count; $i++) {
    $w = $wins.Item($i)
    [void]$sb.AppendLine("  WIN[$i] title='$($w.Current.Name)'")
}
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
