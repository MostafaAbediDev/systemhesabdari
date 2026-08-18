$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '15836' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$btn = Find-ByAId $win 'BtnPersons'
if (-not $btn) { Write-Output "BtnPersons NOT FOUND"; exit 1 }
Click-El $btn
Wait-Ms 2500

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-persons.txt'
Dump-TreeToFile $win $out 7
Write-Output "DUMPED"
