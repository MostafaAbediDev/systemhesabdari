$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19540' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# 1) Unique code input (AId=UniqueCodeInput -> PART_TextBox)
$uc = Find-ByAId $win 'UniqueCodeInput'
if (-not $uc) { Write-Output "UNIQUE CODE INPUT NOT FOUND"; exit 1 }
$ucBox = $uc.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::AutomationIdProperty, 'PART_TextBox')))
if (-not $ucBox) { Write-Output "UC BOX NOT FOUND"; exit 1 }
try {
    $vp = $ucBox.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    $vp.SetValue('QA-EDIT2-500')
    Write-Output "SET UNIQUE CODE = QA-EDIT2-500"
} catch { Write-Output ("UC SET FAILED: " + $_.Exception.Message) }

# 2) Branch name field (Edit with VAL=شعبه ۵۰۰)
$allEdits = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
$nameBox = $null
foreach ($e in $allEdits) {
    try {
        $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        if ($vp.Current.Value -eq 'شعبه ۵۰۰') { $nameBox = $e; break }
    } catch {}
}
if (-not $nameBox) { Write-Output "NAME FIELD NOT FOUND"; exit 1 }
try {
    $vp = $nameBox.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    $vp.SetValue('شعبه ۵۰۰ QA2')
    Write-Output "SET NAME = شعبه ۵۰۰ QA2"
} catch { Write-Output ("NAME SET FAILED: " + $_.Exception.Message) }

Start-Sleep -Milliseconds 400

# 3) Click Save
$save = Find-ByAId $win 'SaveButton'
if (-not $save) { Write-Output "SAVE NOT FOUND"; exit 1 }
Click-El $save
Wait-Ms 2500

# 4) Dump state
$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-after-save3.txt'
Dump-TreeToFile $win $out 6
$saveAgain = Find-ByAId $win 'SaveButton'
Write-Output ("MODAL_STILL_OPEN=" + [bool]$saveAgain)
Write-Output "DONE"
