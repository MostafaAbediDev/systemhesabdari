$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-tree.txt'
"" | Out-File -FilePath $out -Encoding utf8

function Dump-Tree($el, $depth, $maxDepth) {
    if ($depth -gt $maxDepth) { return }
    $name = $el.Current.Name
    $ct = $el.Current.ControlType.ProgrammaticName -replace 'ControlType\.', ''
    $aid = $el.Current.AutomationId
    $cls = $el.Current.ClassName
    $line = ("{0}{1} | {2} | Name=[{3}] | AId=[{4}] | Class=[{5}]" -f ('  ' * $depth), $ct, $depth, $name, $aid, $cls)
    Add-Content -Path $out -Value $line -Encoding utf8
    if ($depth -ge $maxDepth) { return }
    $children = $el.FindAll([System.Windows.Automation.TreeScope]::Children, [System.Windows.Automation.Condition]::TrueCondition)
    foreach ($c in $children) {
        Dump-Tree $c ($depth + 1) $maxDepth
    }
}

$root = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, 19208)
$win = $root.FindFirst([System.Windows.Automation.TreeScope]::Children, $cond)
if (-not $win) {
    Add-Content -Path $out -Value "WINDOW NOT FOUND for PID 19208" -Encoding utf8
} else {
    Add-Content -Path $out -Value ("WINDOW: Name=[" + $win.Current.Name + "] Class=[" + $win.Current.ClassName + "]") -Encoding utf8
    Dump-Tree $win 0 9
}
Write-Output "DONE"
