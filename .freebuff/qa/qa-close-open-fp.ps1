$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# close modal: click انصراف, then No on dialog
$cancel = Find-ByName $win 'انصراف'
if ($cancel) {
    $r = $cancel.Current.BoundingRectangle
    [MouseWin32]::LeftClick([int]($r.X + $r.Width/2), [int]($r.Y + $r.Height/2))
    Wait-Ms 1000
    $dlg = $win.FindFirst([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window)))
    if ($dlg) {
        $no = $dlg.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'No')))
        if ($no) { Click-El $no }
        Wait-Ms 1000
    }
}

# open financial period list
$target = Find-ByName $win 'لیست دوره‌ها'
if (-not $target) {
    $btn = Find-ByAId $win 'BtnBusinessInfo'
    if ($btn) { Click-El $btn; Wait-Ms 700 }
    $target = Find-ByName $win 'لیست دوره‌ها'
}
if (-not $target) { Write-Output "MENU NOT FOUND"; exit 1 }
Click-El $target
Wait-Ms 1500

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-fp-list.txt'
Dump-TreeToFile $win $out 7
Write-Output "DUMPED"
