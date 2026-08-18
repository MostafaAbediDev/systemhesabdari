$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$target = Find-ByName $win 'لیست شرکت‌ها'
if (-not $target) {
    $btn = Find-ByAId $win 'BtnBusinessInfo'
    if ($btn) { Click-El $btn; Wait-Ms 700 }
    $target = Find-ByName $win 'لیست شرکت‌ها'
}
if (-not $target) { Write-Output "MENU NOT FOUND"; exit 1 }
Click-El $target
Wait-Ms 1500

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-company-list.txt'
Dump-TreeToFile $win $out 7
Write-Output "DUMPED"
