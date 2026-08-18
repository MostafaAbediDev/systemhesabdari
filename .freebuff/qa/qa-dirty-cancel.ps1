$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19540' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-dirty-cancel.txt'
"" | Out-File -FilePath $out -Encoding utf8

# 1) Change branch name (append X)
$allEdits = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
$nameBox = $null
foreach ($e in $allEdits) {
    try {
        $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        if ($vp.Current.Value -eq 'شعبه ۵۰۰ QA۲') { $nameBox = $e; break }
    } catch {}
}
if (-not $nameBox) { Add-Content -Path $out -Value "NAME FIELD NOT FOUND" -Encoding utf8; Write-Output "NAME FIELD NOT FOUND"; exit 1 }
try {
    $vp = $nameBox.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    $vp.SetValue('شعبه ۵۰۰ QA۲ DIRTY')
    Add-Content -Path $out -Value "STEP1: NAME CHANGED (dirty)" -Encoding utf8
} catch { Add-Content -Path $out -Value ("STEP1 FAIL: " + $_.Exception.Message) -Encoding utf8 }
Start-Sleep -Milliseconds 400

# 2) Click انصراف (Cancel)
$cancel = Find-ByName $win 'انصراف'
if (-not $cancel) { Add-Content -Path $out -Value "CANCEL NOT FOUND" -Encoding utf8; Write-Output "CANCEL NOT FOUND"; exit 1 }
Click-El $cancel
Wait-Ms 1200

# 3) Check if dirty warning dialog appeared
$dlg = $win.FindFirst([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window)))
$modalStill = Find-ByAId $win 'SaveButton'
if ($dlg) {
    $dlgName = $dlg.Current.Name
    Add-Content -Path $out -Value ("DIRTY_DIALOG=[" + $dlgName + "]") -Encoding utf8
    Dump-TreeToFile $dlg 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-dirty-dialog.txt' 6
} else {
    Add-Content -Path $out -Value ("DIRTY_DIALOG=NONE  MODAL_OPEN=" + [bool]$modalStill) -Encoding utf8
}
Write-Output "DONE"
