$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# find all Custom elements with AId=Root that contain text بروزرسانی
$roots = Find-Els $win ([System.Windows.Automation.AutomationElement]::AutomationIdProperty) 'Root'
$btn = $null
foreach ($root in $roots) {
    $txt = $root.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'بروزرسانی')))
    if ($txt) { $btn = $root; break }
}
if (-not $btn) { Write-Output "REFRESH BTN NOT FOUND"; exit 1 }
$r = $btn.Current.BoundingRectangle
Write-Output ("REFRESH RECT: " + $r.X + "," + $r.Y + " " + $r.Width + "x" + $r.Height)
[MouseWin32]::LeftClick([int]($r.X + $r.Width/2), [int]($r.Y + $r.Height/2))
Wait-Ms 2500

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-refreshed2.txt'
Dump-TreeToFile $win $out 7
$hasQA = Select-String -Path $out -Pattern 'QA-EDIT' -Quiet
$hasShobeh = Select-String -Path $out -Pattern 'شعبه ۵۰۰\]' -Quiet
Write-Output ("HAS_QA=" + $hasQA + " HAS_SHOEBEH=" + $hasShobeh)
Write-Output "DUMPED"
