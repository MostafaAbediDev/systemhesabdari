$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# close any open context menu first
[KeyWin32]::SendKey([KeyWin32]::VK_ESCAPE)
Wait-Ms 400

$grid = Find-ByAId $win 'DataGridView'
if (-not $grid) { Write-Output "GRID NOT FOUND"; exit 1 }
$rows = $grid.FindAll([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::DataItem)))
if ($rows.Count -lt 2) { Write-Output "NOT ENOUGH ROWS"; exit 1 }
$row2 = $rows[1]
$r = $row2.Current.BoundingRectangle
Write-Output ("ROW2 RECT: " + $r.X + "," + $r.Y + " " + $r.Width + "x" + $r.Height)
# right-click on the NAME cell (not the row center) to be safe — col 7 (نام شعبه)
$cellX = [int]($r.X + $r.Width * 0.62)
$cellY = [int]($r.Y + $r.Height / 2)
[MouseWin32]::RightClick($cellX, $cellY)
Write-Output ("RCLICK @ " + $cellX + "," + $cellY)
Wait-Ms 1000

$editBtn = Find-ByAId $win 'EditButton'
if (-not $editBtn) {
    Write-Output "EDIT BUTTON NOT FOUND"
    Dump-TreeToFile $win 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-after-rclick2.txt' 8
    exit 1
}
Click-El $editBtn
Wait-Ms 2500

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-edit-branch.txt'
Dump-TreeToFile $win $out 18
Write-Output "DUMPED"
