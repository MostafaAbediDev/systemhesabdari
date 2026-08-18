$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '15836' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$search = Find-ByAId $win 'SearchBox'
if (-not $search) { Write-Output "SEARCH NOT FOUND"; exit 1 }
try {
    $vp = $search.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    $vp.SetValue('1000000001')
    Write-Output "SET 1000000001"
} catch { Write-Output ("SET FAIL: " + $_.Exception.Message); exit 1 }
Wait-Ms 3500

$grid = Find-ByAId $win 'PersonsGrid'
if (-not $grid) { $grid = Find-ByAId $win 'DataGridView' }
if ($grid) { Dump-TreeToFile $grid 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-nc-rows.txt' 8 }
Write-Output "DUMPED"
