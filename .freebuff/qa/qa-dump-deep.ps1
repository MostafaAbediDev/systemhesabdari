$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-deep.txt'
"" | Out-File -FilePath $out -Encoding utf8

function Dump-Val($e, $depth, $md) {
    if ($depth -gt $md) { return }
    $name = $e.Current.Name
    $ct = $e.Current.ControlType.ProgrammaticName -replace 'ControlType\.', ''
    $aid = $e.Current.AutomationId
    $val = ''
    try {
        $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        $val = '[VAL=' + $vp.Current.Value + ']'
    } catch {}
    $cls = $e.Current.ClassName
    $line = ("{0}{1} | Name=[{2}] | AId=[{3}] | Class=[{4}] {5}" -f ('  ' * $depth), $ct, $name, $aid, $cls, $val)
    Add-Content -Path $out -Value $line -Encoding utf8
    if ($depth -ge $md) { return }
    $children = $e.FindAll([System.Windows.Automation.TreeScope]::Children, [System.Windows.Automation.Condition]::TrueCondition)
    foreach ($c in $children) { Dump-Val $c ($depth + 1) $md }
}
Dump-Val $win 0 20
Write-Output "DUMPED"
