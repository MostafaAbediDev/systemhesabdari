$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# 1) set UniqueCodeInput
$codeInput = Find-ByAId $win 'UniqueCodeInput'
if (-not $codeInput) { Write-Output "UniqueCodeInput NOT FOUND"; exit 1 }
$codeEdit = $codeInput.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
if ($codeEdit) { Set-Text $codeEdit 'QA-EDIT-500'; Wait-Ms 300 }

# 2) set branch name
$edits = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
$target = $null
foreach ($e in $edits) {
    try {
        $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        if ($vp.Current.Value -eq 'شعبه ۵۰۰ ویرایش QA' -or $vp.Current.Value -eq 'شعبه ۵۰۰') { $target = $e; break }
    } catch {}
}
if (-not $target) { Write-Output "NAME FIELD NOT FOUND"; exit 1 }
Set-Text $target 'شعبه ۵۰۰ ویرایش QA'
Wait-Ms 400

# 3) click SaveButton
$save = Find-ByAId $win 'SaveButton'
if (-not $save) { Write-Output "SAVE NOT FOUND"; exit 1 }
$r2 = $save.Current.BoundingRectangle
[MouseWin32]::LeftClick([int]($r2.X + $r2.Width/2), [int]($r2.Y + $r2.Height/2))
Wait-Ms 3000

Dump-TreeToFile $win 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-after-save2.txt' 8
Write-Output "DUMPED"
