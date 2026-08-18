param([string]$Name = 'شخص جدید', [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\invoke_sidebar.txt')
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$found = $null
foreach ($e in $all) {
    if ($e.Current.Name -eq $Name -and $e.Current.ControlType.ProgrammaticName -match 'Button') { $found = $e; break }
}
if (-not $found) { Write-Output "NOT_FOUND name='$Name'"; exit 1 }
try {
    $p = $found.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
    $p.Invoke()
    Write-Output "INVOKED name='$Name' type=$($found.Current.ControlType.ProgrammaticName)"
} catch {
    Write-Output "INVOKE_FAIL $($_.Exception.Message)"
    exit 1
}
Start-Sleep -Milliseconds 2500
$proc.Refresh()
Write-Output "after: alive=$(-not $proc.HasExited) responding=$($proc.Responding)"
# dump current main-window tree to Out
$sb = New-Object System.Text.StringBuilder
$all2 = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
[void]$sb.AppendLine("ELEMENTS=$($all2.Count)")
for ($i = 0; $i -lt $all2.Count; $i++) {
    $e = $all2.Item($i)
    [void]$sb.AppendLine("[$i] $($e.Current.ControlType.ProgrammaticName) | name='$($e.Current.Name)' | id='$($e.Current.AutomationId)' | en=$($e.Current.IsEnabled) | off=$($e.Current.IsOffscreen)")
}
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "SAVED $Out"
