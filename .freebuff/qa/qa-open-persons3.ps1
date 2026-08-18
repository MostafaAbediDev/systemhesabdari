$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '15836' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# ensure submenu open
$target = Find-ByName $win 'لیست اشخاص'
if (-not $target) {
    $btn = Find-ByAId $win 'BtnPersons'
    if ($btn) { Click-El $btn; Wait-Ms 1200 }
    $target = Find-ByName $win 'لیست اشخاص'
}
if (-not $target) {
    Write-Output "STILL NOT FOUND — dumping sidebar"
    $sb = Find-ByAId $win 'Sidebar'
    if ($sb) { Dump-TreeToFile $sb 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-sidebar-now.txt' 8 }
    exit 1
}
Click-El $target
Wait-Ms 3000

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-persons-list.txt'
Dump-TreeToFile $win $out 12
Write-Output "DUMPED"
