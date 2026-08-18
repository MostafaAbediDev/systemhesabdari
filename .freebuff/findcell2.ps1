$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$sb = New-Object System.Text.StringBuilder
for ($i = 0; $i -lt $all.Count; $i++) {
    $e = $all.Item($i)
    $n = $e.Current.Name
    if ($n -eq 'اریاگستر') {
        [void]$sb.AppendLine("[$i] type=$($e.Current.ControlType.ProgrammaticName) rect=$($e.Current.BoundingRectangle)")
    }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_cell2.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
