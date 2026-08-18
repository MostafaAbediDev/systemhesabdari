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
$vp = $search.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
Write-Output ("VALUE=[" + $vp.Current.Value + "]")
