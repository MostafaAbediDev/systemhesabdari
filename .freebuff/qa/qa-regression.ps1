$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# 1) close modal: انصراف -> No
$cancel = Find-ByName $win 'انصراف'
if ($cancel) {
    $r = $cancel.Current.BoundingRectangle
    [MouseWin32]::LeftClick([int]($r.X + $r.Width/2), [int]($r.Y + $r.Height/2))
    Wait-Ms 1000
    $dlg = $win.FindFirst([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window)))
    if ($dlg) {
        $no = $dlg.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'No')))
        if ($no) { Click-El $no; Wait-Ms 800 }
    }
}
Dump-TreeToFile $win 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-tmp1.txt' 4
$modal = Select-String -Path 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-tmp1.txt' -Pattern 'ویرایش دوره' -Quiet
Write-Output ("MODAL_CLOSED=" + (-not $modal))

# 2) BUG-008 regression: right-click on TEXT cells (date cell, title cell, empty-ish cell)
$grid = Find-ByAId $win 'DataGridView'
if (-not $grid) { Write-Output "GRID NOT FOUND"; exit 1 }
$rows = $grid.FindAll([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::DataItem)))
if ($rows.Count -ge 1) {
    $r1 = $rows[0].Current.BoundingRectangle
    # click on the date text cell area (col 5/6 ~ x=0.55) and title cell (x=0.2) and blank right area
    $xs = @(0.25, 0.55, 0.85)
    foreach ($fx in $xs) {
        [MouseWin32]::RightClick([int]($r1.X + $r1.Width * $fx), [int]($r1.Y + $r1.Height/2))
        Wait-Ms 600
        # close any context menu
        [KeyWin32]::SendKey([KeyWin32]::VK_ESCAPE)
        Wait-Ms 300
    }
    Write-Output "RCLICK_TESTS_DONE"
}

# 3) PageSize change: click PageSizeSelector, choose option
$ps = Find-ByAId $win 'PageSizeSelector'
if ($ps) {
    $r2 = $ps.Current.BoundingRectangle
    [MouseWin32]::LeftClick([int]($r2.X + $r2.Width/2), [int]($r2.Y + $r2.Height/2))
    Wait-Ms 1000
    # find popup items (Text elements with numbers)
    $popItems = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Text)))
    $clicked = $false
    foreach ($pi in $popItems) {
        $nm = $pi.Current.Name
        if ($nm -match '^\d+$' -and $nm -ne '۱۵') { 
            $rr = $pi.Current.BoundingRectangle
            if ($rr.Width -gt 0 -and $rr.Height -gt 0) {
                [MouseWin32]::LeftClick([int]($rr.X + $rr.Width/2), [int]($rr.Y + $rr.Height/2))
                Write-Output ("PAGESIZE_CLICKED: " + $nm)
                $clicked = $true
                break
            }
        }
    }
    if (-not $clicked) { Write-Output "PAGESIZE_ITEM_NOT_FOUND" }
    Wait-Ms 1500
}

# 4) Resize window to 1280x720
$hwnd = New-Object System.IntPtr($win.Current.NativeWindowHandle)
Add-Type @"
using System;
using System.Runtime.InteropServices;
public static class WinResize {
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_NOACTIVATE = 0x0010;
}
"@
[WinResize]::SetWindowPos($hwnd, [IntPtr]::Zero, 60, 40, 1280, 720, [WinResize]::SWP_NOZORDER -bor [WinResize]::SWP_NOACTIVATE)
Wait-Ms 1500
Write-Output "RESIZED_TO_1280x720"

# verify app still responding
$proc = Get-Process -Id $taadolPid -ErrorAction SilentlyContinue
Write-Output ("APP_RESPONDING=" + $proc.Responding)

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-regression.txt'
Dump-TreeToFile $win $out 5
Write-Output "DUMPED"
