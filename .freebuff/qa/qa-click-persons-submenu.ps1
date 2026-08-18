$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '15836' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$target = Find-ByName $win 'لیست اشخاص'
if (-not $target) {
    # open persons submenu first
    $btn = Find-ByAId $win 'BtnPersons'
    if ($btn) { Click-El $btn; Wait-Ms 700 }
    $target = Find-ByName $win 'لیست اشخاص'
}
if (-not $target) { Write-Output "MENU ITEM NOT FOUND"; exit 1 }
Click-El $target
Wait-Ms 2500

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-persons-list.txt'
Dump-TreeToFile $win $out 12
Write-Output "DUMPED"
