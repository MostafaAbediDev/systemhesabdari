$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# click refresh (بروزرسانی) button
$refresh = Find-ByName $win 'بروزرسانی'
if (-not $refresh) { Write-Output "REFRESH NOT FOUND"; exit 1 }
$r = $refresh.Current.BoundingRectangle
[MouseWin32]::LeftClick([int]($r.X + $r.Width/2), [int]($r.Y + $r.Height/2))
Wait-Ms 1500

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-refreshed.txt'
Dump-TreeToFile $win $out 7
# show rows 1-3 unique code + name
$lines = Get-Content $out
$idx = 0
foreach ($line in $lines) {
    if ($line -match 'DataItem \| 4') { $idx++ }
    if ($idx -ge 1 -and $idx -le 3 -and ($line -match 'Name=\[' -and $line -match 'Custom | 5')) {
        Write-Output ("ROW" + $idx + ": " + $line.Trim())
    }
}
Write-Output "DUMPED"
