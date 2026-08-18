$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19540' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-valid2.txt'
"" | Out-File -FilePath $out -Encoding utf8

# find name field (value = شعبه ۵۰۰ QA۲)
$allEdits = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
$nameBox = $null
foreach ($e in $allEdits) {
    try {
        $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        if ($vp.Current.Value -eq 'شعبه ۵۰۰ QA۲') { $nameBox = $e; break }
    } catch {}
}
if (-not $nameBox) { Add-Content -Path $out -Value "NAME NOT FOUND" -Encoding utf8; Write-Output "NAME NOT FOUND"; exit 1 }

# empty the name
try {
    $vp = $nameBox.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    $vp.SetValue('')
    Add-Content -Path $out -Value "STEP1: NAME EMPTIED" -Encoding utf8
} catch { Add-Content -Path $out -Value ("STEP1 FAIL: " + $_.Exception.Message) -Encoding utf8 }
Start-Sleep -Milliseconds 400

# click save
$save = Find-ByAId $win 'SaveButton'
if (-not $save) { Add-Content -Path $out -Value "SAVE NOT FOUND" -Encoding utf8; Write-Output "SAVE NOT FOUND"; exit 1 }
Click-El $save
Wait-Ms 2000

# modal still open? save button text?
$modalStill = Find-ByAId $win 'SaveButton'
Add-Content -Path $out -Value ("MODAL_OPEN=" + [bool]$modalStill) -Encoding utf8
if ($modalStill) {
    $btnText = $modalStill.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Text)))
    if ($btnText) { Add-Content -Path $out -Value ("SAVE_BTN_TEXT=[" + $btnText.Current.Name + "]") -Encoding utf8 }
}
# validation message?
$errTexts = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Text)))
foreach ($t in $errTexts) {
    $n = $t.Current.Name
    if ($n -match 'وارد|الزامی|خطا|نامعتبر|اجباری') {
        Add-Content -Path $out -Value ("ERR_TEXT=[" + $n + "]") -Encoding utf8
    }
}
Write-Output "DONE"
