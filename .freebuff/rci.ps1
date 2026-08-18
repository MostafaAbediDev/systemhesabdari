param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32RCI {
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
# right-click on row 1 cell (fresh)
$all = Get-Tree
$found = $null
foreach ($e in $all) { if ($e.Current.Name -eq 'اریاگستر' -and $e.Current.ControlType.ProgrammaticName -match 'Text' -and $e.Current.BoundingRectangle.Y -gt 280) { $found = $e; break } }
if (-not $found) { [void]$sb.AppendLine("CELL_NOT_FOUND"); [IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_rci.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "CELL_NOT_FOUND"; exit 1 }
$r = $found.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32RCI]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 500
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 400
# right-click
[Win32RCI]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 100
[Win32RCI]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
[void]$sb.AppendLine("RCLICK at=$x,$y")
Start-Sleep -Milliseconds 2000
# find menu items
$all2 = Get-Tree
$editBtn = $null
for ($i = 0; $i -lt $all2.Count; $i++) {
    $e = $all2.Item($i)
    if ($e.Current.Name -eq 'ویرایش' -and $e.Current.BoundingRectangle.Width -gt 0) {
        [void]$sb.AppendLine("FOUND[$i] type=$($e.Current.ControlType.ProgrammaticName) id='$($e.Current.AutomationId)' rect=$($e.Current.BoundingRectangle)")
        $editBtn = $e
    }
}
if ($editBtn) {
    $invoked = $false
    $walk = [System.Windows.Automation.TreeWalker]::ControlViewWalker
    $node = $editBtn
    while ($node -ne $null) {
        try {
            $p = $node.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
            $p.Invoke()
            [void]$sb.AppendLine("INVOKED type=$($node.Current.ControlType.ProgrammaticName) name='$($node.Current.Name)'")
            $invoked = $true
            break
        } catch {}
        $node = $walk.GetParent($node)
    }
    if (-not $invoked) {
        $mr = $editBtn.Current.BoundingRectangle
        $mx = [int]($mr.X + $mr.Width/2); $my = [int]($mr.Y + $mr.Height/2)
        [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($mx, $my)
        Start-Sleep -Milliseconds 250
        [Win32RCI]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 80
        [Win32RCI]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
        [void]$sb.AppendLine("CLICKED at=$mx,$my")
    }
} else {
    [void]$sb.AppendLine("MENU_NOT_FOUND")
}
Start-Sleep -Milliseconds 3500
$proc.Refresh()
$all3 = Get-Tree
$fn = 0; $err = 0; $tab = 0
foreach ($e in $all3) {
    if ($e.Current.AutomationId -eq 'FirstNameInput') { $fn++ }
    if ($e.Current.Name -like 'ERROR:*') { $err++ }
    if ($e.Current.AutomationId -eq 'TabInventory') { $tab++ }
}
[void]$sb.AppendLine("RESULT FirstNameInput=$fn TabInventory=$tab ERROR=$err alive=$(-not $proc.HasExited)")
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_rci.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
