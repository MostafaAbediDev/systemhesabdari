param([string]$Cell = 'اریاگستر')
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32RcE2 {
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
# right-click on cell
$all = Get-Tree
$found = $null
foreach ($e in $all) { if ($e.Current.Name -eq $Cell -and $e.Current.ControlType.ProgrammaticName -match 'Text') { $found = $e; break } }
if (-not $found) { Write-Output "CELL_NOT_FOUND"; exit 1 }
$r = $found.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32RcE2]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 250
[Win32RcE2]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32RcE2]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
Write-Output "RCLICK1 at=$x,$y"
Start-Sleep -Milliseconds 800
[Win32RcE2]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32RcE2]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
Write-Output "RCLICK2 at=$x,$y"
Start-Sleep -Milliseconds 2500
# fresh tree: find Edit item
$all2 = Get-Tree
$editIdx = -1
$editRect = $null
for ($i = 0; $i -lt $all2.Count; $i++) {
    $e = $all2.Item($i)
    if ($e.Current.Name -eq 'ویرایش' -and $e.Current.BoundingRectangle.Width -gt 0) {
        if ($editIdx -lt 0) { $editIdx = $i; $editRect = $e.Current.BoundingRectangle }
    }
}
if ($editIdx -lt 0) { Write-Output "EDIT_ITEM_NOT_FOUND"; exit 1 }
$ex = [int]($editRect.X + $editRect.Width/2); $ey = [int]($editRect.Y + $editRect.Height/2)
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($ex, $ey)
Start-Sleep -Milliseconds 250
[Win32RcE2]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32RcE2]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
Write-Output "CLICKED EDIT at=$ex,$ey idx=$editIdx"
Start-Sleep -Milliseconds 3000
$proc.Refresh()
$rootEl3 = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all3 = $rootEl3.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$modal = 0
foreach ($e in $all3) { if ($e.Current.AutomationId -eq 'FirstNameInput') { $modal++ } }
Write-Output "MODAL_OPEN=$($modal -gt 0) alive=$(-not $proc.HasExited) responding=$($proc.Responding)"
