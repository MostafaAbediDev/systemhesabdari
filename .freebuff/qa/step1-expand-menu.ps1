$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 500

$btn = Find-ByAId $win 'BtnBusinessInfo'
if (-not $btn) { Write-Output "BtnBusinessInfo NOT FOUND"; exit 1 }
Click-El $btn
Wait-Ms 1200

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-tree2.txt'
Dump-TreeToFile $win $out 10
Write-Output "DUMPED"
