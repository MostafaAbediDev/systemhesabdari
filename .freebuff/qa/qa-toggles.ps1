$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19540' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-toggles.txt'
"" | Out-File -FilePath $out -Encoding utf8

function Dump-Toggle($aid, $label) {
    $el = Find-ByAId $win $aid
    if (-not $el) { Add-Content -Path $out -Value ("$label : NOT FOUND") -Encoding utf8; return }
    $isOff = $el.FindFirst([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::AutomationIdProperty, 'BtnFirst')))
    $isOn  = $el.FindFirst([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::AutomationIdProperty, 'BtnSecond')))
    foreach ($pair in @(@('BtnFirst', $isOff), @('BtnSecond', $isOn))) {
        $b = $pair[1]
        if (-not $b) { continue }
        $nm = $b.Current.Name
        $chk = '?'
        try {
            $tp = $b.GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern)
            $chk = $tp.Current.ToggleState
        } catch {
            try {
                $sp = $b.GetCurrentPattern([System.Windows.Automation.SelectionItemPattern]::Pattern)
                $chk = 'Sel=' + $sp.Current.IsSelected
            } catch { $chk = 'n/a' }
        }
        Add-Content -Path $out -Value ("$label [$($pair[0])] Name=[$nm] State=$chk") -Encoding utf8
    }
}

Dump-Toggle 'CodeModeToggle' 'CODE_MODE'
Dump-Toggle 'BranchTypeToggle' 'BRANCH_TYPE'
Dump-Toggle 'BranchActiveToggle' 'BRANCH_ACTIVE'

# Also dump selection items inside each toggle
foreach ($aid in @('CodeModeToggle','BranchTypeToggle','BranchActiveToggle')) {
    $el = Find-ByAId $win $aid
    if ($el) {
        $sel = $el.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::RadioButton)))
        Add-Content -Path $out -Value ("$aid RadioButtons: " + $sel.Count) -Encoding utf8
        foreach ($s in $sel) {
            $nm = $s.Current.Name
            $selState = '?'
            try {
                $sp = $s.GetCurrentPattern([System.Windows.Automation.SelectionItemPattern]::Pattern)
                $selState = $sp.Current.IsSelected
            } catch {}
            Add-Content -Path $out -Value ("  RB Name=[$nm] Selected=$selState") -Encoding utf8
        }
    }
}
Write-Output "DUMPED"
