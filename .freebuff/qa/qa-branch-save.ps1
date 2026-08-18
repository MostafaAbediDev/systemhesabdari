$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# Find all Edit elements, locate the one whose value is the branch name
$edits = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
$target = $null
foreach ($e in $edits) {
    try {
        $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        if ($vp.Current.Value -eq 'شعبه ۵۰۰') { $target = $e; break }
    } catch {}
}
if (-not $target) { Write-Output "NAME FIELD NOT FOUND"; exit 1 }
$r = $target.Current.BoundingRectangle
Write-Output ("NAME FIELD RECT: " + $r.X + "," + $r.Y + " " + $r.Width + "x" + $r.Height)
$set = Set-Text $target 'شعبه ۵۰۰ ویرایش QA'
Wait-Ms 400
# verify it took
try {
    $vp = $target.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    Write-Output ("AFTER SET: [" + $vp.Current.Value + "]")
} catch {}

# click SaveButton
$save = Find-ByAId $win 'SaveButton'
if (-not $save) { Write-Output "SAVE BUTTON NOT FOUND"; exit 1 }
$r2 = $save.Current.BoundingRectangle
$cx = [int]($r2.X + $r2.Width / 2)
$cy = [int]($r2.Y + $r2.Height / 2)
Write-Output ("SAVE RECT: " + $cx + "," + $cy)
[MouseWin32]::LeftClick($cx, $cy)
Wait-Ms 2500

Dump-TreeToFile $win 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-after-save.txt' 10
Write-Output "DUMPED"
