$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '15836' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-select1002.txt'
"" | Out-File -FilePath $out -Encoding utf8

$search = Find-ByAId $win 'SearchBox'
if (-not $search) { Add-Content -Path $out -Value "SEARCH NOT FOUND" -Encoding utf8; exit 1 }
try {
    $vp = $search.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    $vp.SetValue('09000001002')
    Add-Content -Path $out -Value "SET 09000001002" -Encoding utf8
} catch { Add-Content -Path $out -Value ("SET FAIL: " + $_.Exception.Message) -Encoding utf8 }
Wait-Ms 3500

$grid = Find-ByAId $win 'PersonsGrid'
if (-not $grid) { $grid = Find-ByAId $win 'DataGridView' }
if (-not $grid) { Add-Content -Path $out -Value "GRID NOT FOUND" -Encoding utf8; exit 1 }
$rows = $grid.FindAll([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::DataItem)))
Add-Content -Path $out -Value ("ROWS=" + $rows.Count) -Encoding utf8
if ($rows.Count -gt 0) {
    $r = $rows[0].Current.BoundingRectangle
    Add-Content -Path $out -Value ("ROW1: " + $r.X + "," + $r.Y + " " + $r.Width + "x" + $r.Height) -Encoding utf8
    [MouseWin32]::LeftClick([int]($r.X + $r.Width/2), [int]($r.Y + $r.Height/2))
    Wait-Ms 1200
    Add-Content -Path $out -Value "CLICKED ROW1 CENTER" -Encoding utf8
}
Dump-TreeToFile $win 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-after-select1002.txt' 8
Write-Output "DONE"
