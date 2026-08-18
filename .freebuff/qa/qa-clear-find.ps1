$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '15836' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-clear-find.txt'
"" | Out-File -FilePath $out -Encoding utf8

$search = Find-ByAId $win 'SearchBox'
if (-not $search) { Add-Content -Path $out -Value "SEARCH NOT FOUND" -Encoding utf8; exit 1 }
try {
    $vp = $search.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    $cur = $vp.Current.Value
    Add-Content -Path $out -Value ("CURRENT_SEARCH=[" + $cur + "]") -Encoding utf8
    $vp.SetValue('')
    Add-Content -Path $out -Value "CLEARED" -Encoding utf8
} catch {
    # click + select all + delete
    $r = $search.Current.BoundingRectangle
    [MouseWin32]::LeftClick([int]($r.X + $r.Width - 10), [int]($r.Y + $r.Height/2))
    Start-Sleep -Milliseconds 200
    [System.Windows.Forms.SendKeys]::SendWait('^a')
    [System.Windows.Forms.SendKeys]::SendWait('{DELETE}')
    Add-Content -Path $out -Value "CLEARED VIA KEYS" -Encoding utf8
}
Wait-Ms 1500

$grid = Find-ByAId $win 'PersonsGrid'
if (-not $grid) { $grid = Find-ByAId $win 'DataGridView' }
if ($grid) {
    $rows = $grid.FindAll([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::DataItem)))
    Add-Content -Path $out -Value ("ROWS=" + $rows.Count) -Encoding utf8
    Dump-TreeToFile $grid 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-grid-rows.txt' 8
}
Write-Output "DONE"
