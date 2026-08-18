$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 400

$btn = Find-ByAId $win 'BtnBusinessInfo'
if ($btn) { Click-El $btn; Wait-Ms 600 }

$target = Find-ByName $win 'لیست شعبه‌ها'
if (-not $target) {
    Write-Output "MENU NOT FOUND"
    Dump-TreeToFile $win 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-current.txt' 6
    exit 1
}
Click-El $target
Wait-Ms 1500
Dump-TreeToFile $win 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-branch-list.txt' 14
Write-Output "DUMPED"
