$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$sb = New-Object System.Text.StringBuilder
# find PersonNameText rect
$nameRect = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'PersonNameText') { $nameRect = $e.Current.BoundingRectangle; break } }
if ($nameRect) { [void]$sb.AppendLine("NAME rect=$nameRect") }
# list all small elements near the header row (y within name row +- 30) with x < name.x (left side in RTL)
foreach ($e in $all) {
    $r = $e.Current.BoundingRectangle
    if ($r.Width -le 0 -or $r.Height -le 0) { continue }
    if ($nameRect -and [math]::Abs($r.Y - $nameRect.Y) -lt 40 -and $r.Width -lt 60 -and $r.Height -lt 40) {
        [void]$sb.AppendLine("  type=$($e.Current.ControlType.ProgrammaticName) name='$($e.Current.Name)' id='$($e.Current.AutomationId)' rect=$r")
    }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_probe.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
