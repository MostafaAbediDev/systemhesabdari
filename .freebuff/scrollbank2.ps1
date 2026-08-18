param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$sb = New-Object System.Text.StringBuilder
$scroller = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'Scroller') { $scroller = $e; break } }
if (-not $scroller) { [void]$sb.AppendLine("NO_SCROLLER"); [IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\bk_scroll.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NO_SCROLLER"; exit 1 }
try {
    $sp = $scroller.GetCurrentPattern([System.Windows.Automation.ScrollPattern]::Pattern)
    [void]$sb.AppendLine("BEFORE: hPercent=$($sp.Current.HorizontalScrollPercent) hView=$([math]::Round($sp.Current.HorizontalViewSize,1)) hMax=$($sp.Current.HorizontalScrollPercent)")
    $sp.Scroll([System.Windows.Automation.ScrollAmount]::LargeIncrement, [System.Windows.Automation.ScrollAmount]::NoAmount)
    Start-Sleep -Milliseconds 1200
    $sp2 = $scroller.GetCurrentPattern([System.Windows.Automation.ScrollPattern]::Pattern)
    [void]$sb.AppendLine("AFTER1: hPercent=$($sp2.Current.HorizontalScrollPercent) hView=$([math]::Round($sp2.Current.HorizontalViewSize,1))")
    $sp2.Scroll([System.Windows.Automation.ScrollAmount]::LargeIncrement, [System.Windows.Automation.ScrollAmount]::NoAmount)
    Start-Sleep -Milliseconds 1200
    $sp3 = $scroller.GetCurrentPattern([System.Windows.Automation.ScrollPattern]::Pattern)
    [void]$sb.AppendLine("AFTER2: hPercent=$($sp3.Current.HorizontalScrollPercent) hView=$([math]::Round($sp3.Current.HorizontalViewSize,1))")
} catch {
    [void]$sb.AppendLine("PATTERN_FAIL: $($_.Exception.Message)")
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\bk_scroll.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
