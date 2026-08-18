$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# find the dialog window (ذخیره تغییرات) — it's a child window of the main window
$dialog = $win.FindFirst([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window)))
if (-not $dialog) { Write-Output "DIALOG NOT FOUND"; exit 1 }
$noBtn = $dialog.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'No')))
if (-not $noBtn) { Write-Output "NO BUTTON NOT FOUND"; exit 1 }
Click-El $noBtn
Wait-Ms 1500

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-after-no.txt'
Dump-TreeToFile $win $out 6
$modal = Select-String -Path $out -Pattern 'ویرایش شعبه' -Quiet
Write-Output ("MODAL_OPEN=" + $modal)
Write-Output "DUMPED"
