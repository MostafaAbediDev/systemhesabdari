param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32RC3 {
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
$all = Get-Tree
$found = $null
foreach ($e in $all) { if ($e.Current.Name -eq 'اریاگستر' -and $e.Current.ControlType.ProgrammaticName -match 'Text' -and $e.Current.BoundingRectangle.Width -gt 0 -and $e.Current.BoundingRectangle.Y -gt 280) { $found = $e; break } }
if (-not $found) { Write-Output "CELL_NOT_FOUND"; exit 1 }
$r = $found.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32RC3]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 500
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 400
[Win32RC3]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 120
[Win32RC3]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 1500
[Win32RC3]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 120
[Win32RC3]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
Write-Output "RCLICK2 at=$x,$y"
Start-Sleep -Milliseconds 2500

# dump: find menu items
$all2 = Get-Tree
$sb = New-Object System.Text.StringBuilder
$editBtn = $null
for ($i = 0; $i -lt $all2.Count; $i++) {
    $e = $all2.Item($i)
    if ($e.Current.Name -eq 'ویرایش' -or $e.Current.Name -eq 'حذف') {
        [void]$sb.AppendLine("MENU[$i] type=$($e.Current.ControlType.ProgrammaticName) name='$($e.Current.Name)' id='$($e.Current.AutomationId)' rect=$($e.Current.BoundingRectangle) en=$($e.Current.IsEnabled)")
        if ($e.Current.Name -eq 'ویرایش' -and -not $editBtn) { $editBtn = $e }
    }
}
if ($editBtn) {
    # try invoke pattern
    $walk = [System.Windows.Automation.TreeWalker]::ControlViewWalker
    $node = $editBtn; $invoked = $false
    while ($node -ne $null) {
        try { $p = $node.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern); $p.Invoke(); [void]$sb.AppendLine("INVOKED parent '$($node.Current.ControlType.ProgrammaticName)'"); $invoked = $true; break } catch {}
        $node = $walk.GetParent($node)
    }
    if (-not $invoked) {
        $mr = $editBtn.Current.BoundingRectangle
        $mx = [int]($mr.X + $mr.Width/2); $my = [int]($mr.Y + $mr.Height/2)
        [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($mx, $my)
        Start-Sleep -Milliseconds 250
        [Win32RC3]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 80
        [Win32RC3]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
        [void]$sb.AppendLine("CLICKED menu item at=$mx,$my")
    }
} else {
    [void]$sb.AppendLine("NO_MENU_ITEMS")
}
Start-Sleep -Milliseconds 3500
$proc.Refresh()
$rootEl3 = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all3 = $rootEl3.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$fn = 0; $err = 0
foreach ($e in $all3) {
    if ($e.Current.AutomationId -eq 'FirstNameInput') { $fn++ }
    if ($e.Current.Name -like 'ERROR:*') { $err++ }
}
[void]$sb.AppendLine("RESULT FirstNameInput=$fn ERROR=$err alive=$(-not $proc.HasExited)")
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_rctx.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> fx_rctx.txt"
