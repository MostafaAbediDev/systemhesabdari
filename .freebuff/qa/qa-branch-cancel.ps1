$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# restore name field
$edits = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
$target = $null
foreach ($e in $edits) {
    try {
        $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        if ($vp.Current.Value -eq '') { $target = $e; break }
    } catch {}
}
if ($target) { Set-Text $target 'شعبه ۵۰۰ ویرایش QA'; Wait-Ms 300 }

# click cancel (انصراف Text element)
$cancel = Find-ByName $win 'انصراف'
if (-not $cancel) { Write-Output "CANCEL NOT FOUND"; exit 1 }
$r = $cancel.Current.BoundingRectangle
Write-Output ("CANCEL RECT: " + $r.X + "," + $r.Y + " " + $r.Width + "x" + $r.Height)
[MouseWin32]::LeftClick([int]($r.X + $r.Width/2), [int]($r.Y + $r.Height/2))
Wait-Ms 1200

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-branch-cancel.txt'
Dump-TreeToFile $win $out 10
Write-Output "DUMPED"
