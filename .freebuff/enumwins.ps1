$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$rootEl = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("WINCOUNT=$($wins.Count)")
for ($i = 0; $i -lt $wins.Count; $i++) {
    $w = $wins.Item($i)
    $r = $w.Current.BoundingRectangle
    [void]$sb.AppendLine("WIN[$i] title='$($w.Current.Name)' cls='$($w.Current.ClassName)' rect=$($r.X),$($r.Y),$($r.Width)x$($r.Height) vis=$($w.Current.IsOffscreen -eq $false)")
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\verify_wins.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
