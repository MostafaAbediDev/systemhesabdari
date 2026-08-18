$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# find name field (value contains ویرایش QA)
$edits = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
$target = $null
foreach ($e in $edits) {
    try {
        $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        if ($vp.Current.Value -like '*ویرایش QA*') { $target = $e; break }
    } catch {}
}
if (-not $target) { Write-Output "NAME FIELD NOT FOUND"; exit 1 }
Set-Text $target ''
Wait-Ms 400

$save = Find-ByAId $win 'SaveButton'
if (-not $save) { Write-Output "SAVE NOT FOUND"; exit 1 }
$r2 = $save.Current.BoundingRectangle
[MouseWin32]::LeftClick([int]($r2.X + $r2.Width/2), [int]($r2.Y + $r2.Height/2))
Wait-Ms 1500

# check: modal still open? button text? toast?
$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-branch-validation.txt'
Dump-TreeToFile $win $out 8
$modalOpen = (Select-String -Path $out -Pattern 'ویرایش شعبه' -Quiet)
$btnTxt = ''
$sb = Find-ByAId $win 'SaveButton'
if ($sb) {
    $txt = $sb.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Text)))
    if ($txt) { $btnTxt = $txt.Current.Name }
}
Write-Output ("MODAL_OPEN=" + $modalOpen + " BUTTON=[" + $btnTxt + "]")
Write-Output "DUMPED"
