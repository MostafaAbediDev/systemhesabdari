$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# find name field (value = ایران خودرد)
$edits = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
$target = $null
foreach ($e in $edits) {
    try {
        $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        if ($vp.Current.Value -eq 'ایران خودرد') { $target = $e; break }
    } catch {}
}
if (-not $target) { Write-Output "COMPANY NAME FIELD NOT FOUND"; exit 1 }
Set-Text $target 'ایران خودرد QA'
Wait-Ms 400

$save = Find-ByAId $win 'SaveButton'
if (-not $save) { Write-Output "SAVE NOT FOUND"; exit 1 }
$r2 = $save.Current.BoundingRectangle
[MouseWin32]::LeftClick([int]($r2.X + $r2.Width/2), [int]($r2.Y + $r2.Height/2))
Wait-Ms 3000

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-company-after-save.txt'
Dump-TreeToFile $win $out 6
$modal = Select-String -Path $out -Pattern 'ویرایش شرکت' -Quiet
Write-Output ("MODAL_OPEN=" + $modal)
$hasQA = Select-String -Path $out -Pattern 'ایران خودرد QA' -Quiet
Write-Output ("GRID_HAS_QA_NAME=" + $hasQA)
Write-Output "DUMPED"
