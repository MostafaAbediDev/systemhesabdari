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
if ($search) {
    try {
        $vp = $search.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        $vp.SetValue('')
        Write-Output "CLEARED"
    } catch { Write-Output ("CLEAR FAIL: " + $_.Exception.Message) }
} else { Write-Output "SEARCH NOT FOUND" }
Wait-Ms 3000

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-sum-clear.txt'
Dump-TreeToFile $win $out 6
Write-Output "DUMPED"
