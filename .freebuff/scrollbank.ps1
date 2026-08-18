param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$sb = New-Object System.Text.StringBuilder
$scroll = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'HorizontalScrollBar') { $scroll = $e; break } }
if (-not $scroll) { [void]$sb.AppendLine("NO_HSCROLLBAR"); [IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\bk_scroll.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NO_HSCROLLBAR"; exit 1 }
$sp = $scroll.GetCurrentPattern([System.Windows.Automation.ScrollPattern]::Pattern)
[void]$sb.AppendLine("HSCROLL rect=$($scroll.Current.BoundingRectangle)")
[void]$sb.AppendLine("BEFORE: hPercent=$($sp.Current.HorizontalScrollPercent) hView=$($sp.Current.HorizontalViewSize)")
$sp.Scroll([System.Windows.Automation.ScrollAmount]::LargeIncrement, [System.Windows.Automation.ScrollAmount]::NoAmount)
Start-Sleep -Milliseconds 1200
$sp2 = $scroll.GetCurrentPattern([System.Windows.Automation.ScrollPattern]::Pattern)
[void]$sb.AppendLine("AFTER1: hPercent=$($sp2.Current.HorizontalScrollPercent) hView=$($sp2.Current.HorizontalViewSize)")
$sp2.Scroll([System.Windows.Automation.ScrollAmount]::LargeIncrement, [System.Windows.Automation.ScrollAmount]::NoAmount)
Start-Sleep -Milliseconds 1200
$sp3 = $scroll.GetCurrentPattern([System.Windows.Automation.ScrollPattern]::Pattern)
[void]$sb.AppendLine("AFTER2: hPercent=$($sp3.Current.HorizontalScrollPercent) hView=$($sp3.Current.HorizontalViewSize)")
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\bk_scroll.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
