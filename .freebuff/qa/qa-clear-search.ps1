$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$sb = Find-ByAId $win 'BranchSearchBox'
if ($sb) {
    $v = ''
    try {
        $vp = $sb.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        $v = $vp.Current.Value
    } catch {}
    Write-Output ("SEARCHBOX WAS: [" + $v + "]")
    Set-Text $sb ''
    Wait-Ms 1200
} else {
    Write-Output "SEARCHBOX NOT FOUND"
}

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-after-clear.txt'
Dump-TreeToFile $win $out 6
$info = Select-String -Path $out -Pattern 'نمایش ' | Select-Object -Last 1
Write-Output ("INFO: " + $info)
Write-Output "DUMPED"
