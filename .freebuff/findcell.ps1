param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$sb = New-Object System.Text.StringBuilder
for ($i = 0; $i -lt $all.Count; $i++) {
    $e = $all.Item($i)
    if ($e.Current.Name -eq 'اریاگستر') {
        [void]$sb.AppendLine("[$i] type=$($e.Current.ControlType.ProgrammaticName) id='$($e.Current.AutomationId)' rect=$($e.Current.BoundingRectangle) off=$($e.Current.IsOffscreen)")
    }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_cell.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
