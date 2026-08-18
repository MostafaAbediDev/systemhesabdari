$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$save = Find-ByAId $win 'SaveButton'
if (-not $save) { Write-Output "SAVE NOT FOUND"; exit 1 }
$r2 = $save.Current.BoundingRectangle
$cx = [int]($r2.X + $r2.Width/2)
$cy = [int]($r2.Y + $r2.Height/2)

# rapid double save
[MouseWin32]::LeftClick($cx, $cy)
Wait-Ms 80
[MouseWin32]::LeftClick($cx, $cy)
Wait-Ms 2500

$out1 = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-doublesave.txt'
Dump-TreeToFile $win $out1 6
$modal1 = Select-String -Path $out1 -Pattern 'ویرایش شرکت' -Quiet
Write-Output ("AFTER_DOUBLE: MODAL_OPEN=" + $modal1)

# if modal closed, reopen it (row 3) for validation test
if (-not $modal1) {
    [KeyWin32]::SendKey([KeyWin32]::VK_ESCAPE)
    Wait-Ms 300
    $grid = Find-ByAId $win 'DataGridView'
    if ($grid) {
        $rows = $grid.FindAll([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::DataItem)))
        if ($rows.Count -ge 3) {
            $r = $rows[2].Current.BoundingRectangle
            [MouseWin32]::RightClick([int]($r.X + $r.Width * 0.6), [int]($r.Y + $r.Height/2))
            Wait-Ms 900
            $editBtn = Find-ByAId $win 'EditButton'
            if ($editBtn) { Click-El $editBtn; Wait-Ms 2000 }
        }
    }
}

# validation: clear the company name field and save
$edits = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
$target = $null
foreach ($e in $edits) {
    try {
        $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        if ($vp.Current.Value -like '*ایران خودرد*') { $target = $e; break }
    } catch {}
}
if ($target) { Set-Text $target ''; Wait-Ms 300 }

$save2 = Find-ByAId $win 'SaveButton'
if ($save2) {
    $r3 = $save2.Current.BoundingRectangle
    [MouseWin32]::LeftClick([int]($r3.X + $r3.Width/2), [int]($r3.Y + $r3.Height/2))
    Wait-Ms 1500
}

$out2 = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-validation.txt'
Dump-TreeToFile $win $out2 6
$modal2 = Select-String -Path $out2 -Pattern 'ویرایش شرکت' -Quiet
Write-Output ("AFTER_VALID: MODAL_OPEN=" + $modal2)
Write-Output "DONE"
