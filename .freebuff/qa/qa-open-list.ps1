param([string]$MenuName, [string]$OutFile)
$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 400

# ensure BusinessInfo submenu expanded
$btn = Find-ByAId $win 'BtnBusinessInfo'
if ($btn) { Click-El $btn; Wait-Ms 600 }

$target = Find-ByName $win $MenuName
if (-not $target) {
    Write-Output ("MENU NOT FOUND: " + $MenuName)
    $dump = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-current.txt'
    Dump-TreeToFile $win $dump 6
    exit 1
}
Click-El $target
Wait-Ms 1500

$outPath = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\' + $OutFile
Dump-TreeToFile $win $outPath 12
Write-Output ("DUMPED -> " + $OutFile)
