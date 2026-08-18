$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$sb = New-Object System.Text.StringBuilder
foreach ($e in $all) {
    $r = $e.Current.BoundingRectangle
    if ($r.Width -le 0 -or $r.Height -le 0) { continue }
    if ($r.Y -ge 240 -and $r.Y -le 275 -and $r.X -ge 1310 -and $r.X -le 1570) {
        [void]$sb.AppendLine("  type=$($e.Current.ControlType.ProgrammaticName) name='$($e.Current.Name)' id='$($e.Current.AutomationId)' rect=$r")
    }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_probe2.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
