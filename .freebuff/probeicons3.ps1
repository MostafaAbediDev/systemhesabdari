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
    # everything on the header band of the right panel: y 245-275, x 1300-1600
    if ($r.Y -ge 245 -and $r.Y -le 275 -and $r.X -ge 1300 -and $r.X -le 1600) {
        [void]$sb.AppendLine("  type=$($e.Current.ControlType.ProgrammaticName) name='$($e.Current.Name)' id='$($e.Current.AutomationId)' rect=$r")
    }
}
# also the CloseIcon / DeleteIcon / CollapseIcon named elements anywhere
foreach ($e in $all) {
    if ($e.Current.AutomationId -match 'Icon|Edit|Delete|Close|Collapse|Btn') {
        $r = $e.Current.BoundingRectangle
        if ($r.Width -gt 0) { [void]$sb.AppendLine("  NAMED: id='$($e.Current.AutomationId)' name='$($e.Current.Name)' rect=$r") }
    }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_probe3.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
