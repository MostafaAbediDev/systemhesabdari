$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19208' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300
[KeyWin32]::SendKey([KeyWin32]::VK_ESCAPE)
Wait-Ms 300

$grid = Find-ByAId $win 'DataGridView'
if (-not $grid) { Write-Output "GRID NOT FOUND"; exit 1 }
$rows = $grid.FindAll([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::DataItem)))
if ($rows.Count -lt 1) { Write-Output "NO ROWS"; exit 1 }
$row1 = $rows[0]
$r = $row1.Current.BoundingRectangle
[MouseWin32]::RightClick([int]($r.X + $r.Width * 0.5), [int]($r.Y + $r.Height/2))
Wait-Ms 900
$editBtn = Find-ByAId $win 'EditButton'
if (-not $editBtn) { Write-Output "EDIT NOT FOUND"; exit 1 }
Click-El $editBtn
Wait-Ms 2000

# dump loaded values (persistence check)
$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-fp-reopen.txt'
"" | Out-File -FilePath $out -Encoding utf8
function Dump-Val($e, $depth, $md) {
    if ($depth -gt $md) { return }
    $name = $e.Current.Name
    $ct = $e.Current.ControlType.ProgrammaticName -replace 'ControlType\.', ''
    $aid = $e.Current.AutomationId
    $val = ''
    try {
        $vp = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        $val = '[VAL=' + $vp.Current.Value + ']'
    } catch {}
    $line = ("{0}{1} | Name=[{2}] | AId=[{3}] {4}" -f ('  ' * $depth), $ct, $name, $aid, $val)
    Add-Content -Path $out -Value $line -Encoding utf8
    if ($depth -ge $md) { return }
    $children = $e.FindAll([System.Windows.Automation.TreeScope]::Children, [System.Windows.Automation.Condition]::TrueCondition)
    foreach ($c in $children) { Dump-Val $c ($depth + 1) $md }
}
Dump-Val $win 0 10

# now validation test: set end date year to 1400 (before start 1409)
$endPicker = Find-ByAId $win 'EndDatePicker'
if (-not $endPicker) { Write-Output "ENDPICKER NOT FOUND"; exit 1 }
$yearEdit = $endPicker.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::AutomationIdProperty, 'YearTextBox')))
if ($yearEdit) { Set-Text $yearEdit '1400'; Wait-Ms 400 }

$save = Find-ByAId $win 'SaveButton'
if ($save) {
    $r2 = $save.Current.BoundingRectangle
    [MouseWin32]::LeftClick([int]($r2.X + $r2.Width/2), [int]($r2.Y + $r2.Height/2))
    Wait-Ms 1500
}
$out2 = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-fp-validfail.txt'
Dump-TreeToFile $win $out2 6
$modal2 = Select-String -Path $out2 -Pattern 'ویرایش دوره مالی' -Quiet
Write-Output ("PERSIST_MODAL_OPEN=True(was-open) AFTER_BAD_VALID: MODAL_OPEN=" + $modal2)
Write-Output "DONE"
