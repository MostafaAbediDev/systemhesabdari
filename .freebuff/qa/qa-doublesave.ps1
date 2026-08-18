$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19540' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# 1) restore name
$allEdits = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
$nameBox = $null
foreach ($e in $allEdits) {
    try {
        $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        if ($vp.Current.Value -eq '') {
            # empty name box candidates - skip placeholders; keep first empty edit under main card
            if (-not $nameBox) { $nameBox = $e }
        }
    } catch {}
}
# The name field was emptied; find it via its label proximity is complex. Instead find the edit whose
# original value we know: it is the only empty Edit among the main card. Use fallback: find by rect near نام شعبه label.
if (-not $nameBox) { Write-Output "EMPTY NAME BOX NOT FOUND"; exit 1 }

# Safer: find the name field by matching against UniqueCodeInput sibling pattern: locate all Edits with empty value and pick the one below 'نام شعبه *' text
$labels = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Text)))
$nameLabel = $null
foreach ($l in $labels) { if ($l.Current.Name -eq 'نام شعبه *') { $nameLabel = $l; break } }
if (-not $nameLabel) { Write-Output "NAME LABEL NOT FOUND"; exit 1 }
$lr = $nameLabel.Current.BoundingRectangle
$allEdits2 = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
$nameBox = $null
foreach ($e in $allEdits2) {
    $er = $e.Current.BoundingRectangle
    if ($er.Y -ge $lr.Y -and $er.Y -lt ($lr.Y + 80) -and $er.X -ge $lr.X) { $nameBox = $e }
}
if (-not $nameBox) { Write-Output "NAME BOX BY RECT NOT FOUND"; exit 1 }
try {
    $vp = $nameBox.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    $vp.SetValue('شعبه ۵۰۰ QA۲')
    Write-Output "NAME RESTORED"
} catch { Write-Output ("RESTORE FAIL: " + $_.Exception.Message); exit 1 }
Start-Sleep -Milliseconds 400

# 2) rapid triple save
$save = Find-ByAId $win 'SaveButton'
if (-not $save) { Write-Output "SAVE NOT FOUND"; exit 1 }
$r = $save.Current.BoundingRectangle
$cx = [int]($r.X + $r.Width/2)
$cy = [int]($r.Y + $r.Height/2)
[MouseWin32]::LeftClick($cx, $cy)
Start-Sleep -Milliseconds 80
[MouseWin32]::LeftClick($cx, $cy)
Start-Sleep -Milliseconds 80
[MouseWin32]::LeftClick($cx, $cy)
Wait-Ms 2000

# 3) state
$modalStill = Find-ByAId $win 'SaveButton'
Write-Output ("MODAL_OPEN_AFTER_TRIPLE=" + [bool]$modalStill)
$proc = Get-Process -Id $taadolPid -ErrorAction SilentlyContinue
Write-Output ("APP_ALIVE=" + [bool]$proc + " RESP=" + $proc.Responding)
Write-Output "DONE"
