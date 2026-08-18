$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$exitBtn = Find-ByName $win 'خروج از برنامه'
if (-not $exitBtn) { Write-Output "EXIT NOT FOUND"; exit 1 }
$r = $exitBtn.Current.BoundingRectangle
[MouseWin32]::LeftClick([int]($r.X + $r.Width/2), [int]($r.Y + $r.Height/2))
Wait-Ms 1200

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-exit.txt'
Dump-TreeToFile $win $out 4
$dlg = Select-String -Path $out -Pattern 'Window | 1' -Quiet
Write-Output ("CONFIRM_DIALOG_SHOWN=" + $dlg)

if ($dlg) {
    # find the dialog and its buttons
    $childWin = $win.FindFirst([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window)))
    if ($childWin) {
        Write-Output ("DIALOG_TITLE=[" + $childWin.Current.Name + "]")
        $btns = $childWin.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Button)))
        foreach ($b in $btns) { Write-Output ("DLG_BTN: [" + $b.Current.Name + "]") }
        # click No/Cancel to stay
        $stay = $childWin.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'No')))
        if (-not $stay) { $stay = $childWin.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'Cancel'))) }
        if ($stay) { Click-El $stay; Wait-Ms 1000 }
    }
}

$proc = Get-Process -Id $taadolPid -ErrorAction SilentlyContinue
Write-Output ("APP_STILL_RUNNING=" + [bool]$proc)
Write-Output "DONE"
