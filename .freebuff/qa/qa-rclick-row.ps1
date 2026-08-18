$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$grid = Find-ByAId $win 'DataGridView'
if (-not $grid) { Write-Output "GRID NOT FOUND"; exit 1 }

$rows = $grid.FindAll([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::DataItem)))
Write-Output ("ROWS FOUND: " + $rows.Count)
if ($rows.Count -eq 0) {
    # maybe grid rows are deeper; dump grid subtree
    Dump-TreeToFile $grid 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-grid-subtree.txt' 8
    exit 1
}
$first = $rows[0]
$r = $first.Current.BoundingRectangle
Write-Output ("ROW1 RECT: " + $r.X + "," + $r.Y + " " + $r.Width + "x" + $r.Height)
RightClick-El $first
Wait-Ms 900

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-ctxmenu.txt'
Dump-TreeToFile $win $out 10
Write-Output "DUMPED"
