param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$sb = New-Object System.Text.StringBuilder
foreach ($e in $all) {
    $id = $e.Current.AutomationId
    if ($id -match 'DetailPanel|PersonName|ActiveStatus|ContentScroll|CloseIcon|TabBasic|TabBank|TabHistory|CategoryText') {
        [void]$sb.AppendLine("id='$id' name='$($e.Current.Name)' rect=$($e.Current.BoundingRectangle)")
    }
}
# dump ALL elements between panel header and grid (rect y between 200 and 300, x > 1400)
[void]$sb.AppendLine("--- elements y in [230,300] x>1300 ---")
foreach ($e in $all) {
    $r = $e.Current.BoundingRectangle
    if ($r.Y -ge 230 -and $r.Y -le 300 -and $r.X -ge 1300 -and $r.Width -gt 0) {
        [void]$sb.AppendLine("type=$($e.Current.ControlType.ProgrammaticName) name='$($e.Current.Name)' id='$($e.Current.AutomationId)' rect=$($r)")
    }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_panel.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
