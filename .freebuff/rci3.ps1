$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32RCI3 {
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
# 1) right-click first data row
$all = Get-Tree
$row = $null
foreach ($e in $all) {
    if ($e.Current.ControlType.ProgrammaticName -eq 'ControlType.DataItem' -and $e.Current.BoundingRectangle.Width -gt 50) { $row = $e; break }
}
if (-not $row) { [void]$sb.AppendLine("ROW_NOT_FOUND"); [IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_rci3.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "ROW_NOT_FOUND"; exit 1 }
$r = $row.Current.BoundingRectangle
$x = [int]($r.X + $r.Width * 0.6); $y = [int]($r.Y + $r.Height / 2)
[void][Win32RCI3]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 300
[Win32RCI3]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 100
[Win32RCI3]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
[void]$sb.AppendLine("RCLICK at=$x,$y")
Start-Sleep -Milliseconds 2200
# 2) find Edit menu item text and mouse-click it
$all2 = Get-Tree
$editText = $null
for ($i = 0; $i -lt $all2.Count; $i++) {
    $e = $all2.Item($i)
    if ($e.Current.Name -eq 'ویرایش' -and $e.Current.BoundingRectangle.Width -gt 0) {
        [void]$sb.AppendLine("MENUITEM[$i] type=$($e.Current.ControlType.ProgrammaticName) rect=$($e.Current.BoundingRectangle)")
        if ($e.Current.ControlType.ProgrammaticName -eq 'ControlType.Text') { $editText = $e; break }
    }
}
if (-not $editText) {
    [void]$sb.AppendLine("MENU_TEXT_NOT_FOUND")
} else {
    $mr = $editText.Current.BoundingRectangle
    $mx = [int]($mr.X + $mr.Width / 2); $my = [int]($mr.Y + $mr.Height / 2)
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($mx, $my)
    Start-Sleep -Milliseconds 200
    [Win32RCI3]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 60
    [Win32RCI3]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
    [void]$sb.AppendLine("CLICKED_MENU at=$mx,$my")
}
Start-Sleep -Milliseconds 4000
$proc.Refresh()
$all3 = Get-Tree
$fn = 0; $err = 0; $tab = 0; $tbl = 0
foreach ($e in $all3) {
    if ($e.Current.AutomationId -eq 'FirstNameInput') { $fn++ }
    if ($e.Current.Name -like 'ERROR:*') { $err++ }
    if ($e.Current.AutomationId -eq 'TabInventory') { $tab++ }
    if ($e.Current.AutomationId -eq 'BankAccountsTable') { $tbl++ }
}
[void]$sb.AppendLine("RESULT FirstNameInput=$fn TabInventory=$tab BankAccountsTable=$tbl ERROR=$err alive=$(-not $proc.HasExited)")
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_rci3.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
