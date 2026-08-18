param([string]$Cell = 'QA FinalTest۲', [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\ctxdel3_result.txt')
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Cd3 {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$sb = New-Object System.Text.StringBuilder
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$hwnd = $proc.MainWindowHandle

function Get-Tree {
    $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
    return $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
}

$all = Get-Tree
$target = $null
foreach ($e in $all) {
    if ($e.Current.Name -eq $Cell -and $e.Current.ControlType.ProgrammaticName -match 'Text') { $target = $e; break }
}
if (-not $target) { [void]$sb.AppendLine("ROW_TEXT_NOT_FOUND '$Cell'"); [IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "ROW_TEXT_NOT_FOUND"; exit 1 }
$r = $target.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32Cd3]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 200
[Win32Cd3]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32Cd3]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
[void]$sb.AppendLine("RCLICK '$Cell' at=$x,$y")
Start-Sleep -Milliseconds 2000

$all2 = Get-Tree
$del = $null
foreach ($e in $all2) {
    if ($e.Current.Name -eq 'حذف' -and $e.Current.AutomationId -ne 'BtnText' -and $e.Current.BoundingRectangle.Width -gt 0) { $del = $e; break }
}
if (-not $del) {
    [void]$sb.AppendLine("CTX_DELETE_NOT_FOUND")
    # dump what 'حذف' items exist
    foreach ($e in $all2) { if ($e.Current.Name -eq 'حذف') { [void]$sb.AppendLine("  حذف item: id=$($e.Current.AutomationId) rect=$($e.Current.BoundingRectangle)") } }
} else {
    $r2 = $del.Current.BoundingRectangle
    $x2 = [int]($r2.X + $r2.Width/2); $y2 = [int]($r2.Y + $r2.Height/2)
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x2, $y2)
    Start-Sleep -Milliseconds 200
    [Win32Cd3]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 60
    [Win32Cd3]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
    [void]$sb.AppendLine("CLICKED ctx حذف at=$x2,$y2")
}
Start-Sleep -Milliseconds 2500
$proc.Refresh()
[void]$sb.AppendLine("after: alive=$(-not $proc.HasExited) responding=$($proc.Responding) memMB=$([math]::Round($proc.WorkingSet64/1MB))")

$rootEl3 = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl3.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
for ($i = 0; $i -lt $wins.Count; $i++) { [void]$sb.AppendLine("WIN[$i] title='$($wins.Item($i).Current.Name)'") }

$all4 = Get-Tree
$hits = @()
foreach ($e in $all4) {
    $n = $e.Current.Name
    if ($n -and ($n -match 'مطمئن|حذف شود|تأیید|تایید|بله|خیر')) { $hits += "$($e.Current.ControlType.ProgrammaticName):'$n':id=$($e.Current.AutomationId):en=$($e.Current.IsEnabled):off=$($e.Current.IsOffscreen)" }
}
if ($hits.Count -eq 0) { [void]$sb.AppendLine("NO_CONFIRM_TEXT") } else { [void]$sb.AppendLine("CONFIRM_TEXT: $($hits -join ' | ')"); foreach ($h in $hits) { if ($h -match 'مطمئن') { [void]$sb.AppendLine("DELETECONFIRM_FOUND") } } }
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
