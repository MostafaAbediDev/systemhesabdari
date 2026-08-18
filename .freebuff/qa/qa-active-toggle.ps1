$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19540' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$el = Find-ByAId $win 'BranchActiveToggle'
if (-not $el) { Write-Output "NOT FOUND"; exit 1 }
Dump-TreeToFile $el 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-active-toggle.txt' 8
# Also try TogglePattern on all descendants
$tgls = $el.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Button)))
$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-active-toggle.txt'
Add-Content -Path $out -Value ("BUTTONS=" + $tgls.Count) -Encoding utf8
foreach ($t in $tgls) {
    $state = '?'
    try {
        $tp = $t.GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern)
        $state = $tp.Current.ToggleState
    } catch {}
    Add-Content -Path $out -Value ("  BTN Name=[" + $t.Current.Name + "] AId=[" + $t.Current.AutomationId + "] Toggle=" + $state) -Encoding utf8
}
Write-Output "DUMPED"
