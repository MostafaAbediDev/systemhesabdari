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
    if ($e.Current.AutomationId -match '^tab') {
        $state = 'N/A'
        try {
            $tp = $e.GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern)
            $state = $tp.Current.ToggleState.ToString()
        } catch {}
        [void]$sb.AppendLine("TAB id=$($e.Current.AutomationId) name='$($e.Current.Name)' toggle=$state rect=$($e.Current.BoundingRectangle)")
    }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\sm2_tabs.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
