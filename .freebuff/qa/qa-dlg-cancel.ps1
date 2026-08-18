$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19540' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$dlg = $win.FindFirst([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window)))
if (-not $dlg) { Write-Output "DIALOG NOT FOUND"; exit 1 }
$cancel = $dlg.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'No')))
if (-not $cancel) { Write-Output "CANCEL BTN NOT FOUND"; exit 1 }
Click-El $cancel
Wait-Ms 1200

$modalStill = Find-ByAId $win 'SaveButton'
$nameStill = $false
if ($modalStill) {
    $allEdits = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
    foreach ($e in $allEdits) {
        try {
            $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
            if ($vp.Current.Value -eq 'شعبه ۵۰۰ QA۲ DIRTY') { $nameStill = $true; break }
        } catch {}
    }
}
Write-Output ("MODAL_OPEN=" + [bool]$modalStill)
Write-Output ("DIRTY_VALUE_KEPT=" + $nameStill)
