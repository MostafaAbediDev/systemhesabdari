$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '15836' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

$out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-search-person.txt'
"" | Out-File -FilePath $out -Encoding utf8

# find the search box
$search = $null
foreach ($aid in @('PersonSearchBox', 'SearchBox')) {
    $search = Find-ByAId $win $aid
    if ($search) { break }
}
if (-not $search) {
    # find any Edit control near the top of the view
    $edits = $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))
    foreach ($e in $edits) {
        $aid = $e.Current.AutomationId
        if ($aid -match 'Search|search') { $search = $e; break }
    }
}
if (-not $search) { Add-Content -Path $out -Value "SEARCH BOX NOT FOUND" -Encoding utf8; Write-Output "SEARCH NOT FOUND"; exit 1 }

Add-Content -Path $out -Value ("SEARCH AId=[" + $search.Current.AutomationId + "]") -Encoding utf8
try {
    $vp = $search.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    $vp.SetValue('1000000001')
    Add-Content -Path $out -Value "SET 1000000001" -Encoding utf8
} catch {
    # fallback: click and type
    $r = $search.Current.BoundingRectangle
    [MouseWin32]::LeftClick([int]($r.X + $r.Width/2), [int]($r.Y + $r.Height/2))
    Start-Sleep -Milliseconds 300
    [System.Windows.Forms.SendKeys]::SendWait('1000000001')
    Add-Content -Path $out -Value "TYPED via SendKeys" -Encoding utf8
}
Wait-Ms 1500

$grid = Find-ByAId $win 'DataGridView'
if ($grid) {
    $rows = $grid.FindAll([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::DataItem)))
    Add-Content -Path $out -Value ("ROWS_AFTER_SEARCH=" + $rows.Count) -Encoding utf8
    if ($rows.Count -gt 0) {
        $r = $rows[0].Current.BoundingRectangle
        Add-Content -Path $out -Value ("ROW1 RECT: " + $r.X + "," + $r.Y + " " + $r.Width + "x" + $r.Height) -Encoding utf8
        # select the row (click on its checkbox/row)
        [MouseWin32]::LeftClick([int]($r.X + 20), [int]($r.Y + $r.Height/2))
        Wait-Ms 800
        Add-Content -Path $out -Value "CLICKED ROW1" -Encoding utf8
    }
}
Write-Output "DONE"
