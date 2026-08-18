param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$rootEl = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("WINDOWS=$($wins.Count)")
for ($i = 0; $i -lt $wins.Count; $i++) {
    $w = $wins.Item($i)
    [void]$sb.AppendLine("WIN[$i] name='$($w.Current.Name)' type=$($w.Current.ControlType.ProgrammaticName) rect=$($w.Current.BoundingRectangle)")
    $all = $w.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
    [void]$sb.AppendLine("  ELEMENTS=$($all.Count)")
    for ($j = 0; $j -lt $all.Count; $j++) {
        $e = $all.Item($j)
        [void]$sb.AppendLine("  [$j] $($e.Current.ControlType.ProgrammaticName) | name='$($e.Current.Name)' | id='$($e.Current.AutomationId)' | en=$($e.Current.IsEnabled) | off=$($e.Current.IsOffscreen)")
    }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\sm2_wins.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DUMPED windows=$($wins.Count)"
