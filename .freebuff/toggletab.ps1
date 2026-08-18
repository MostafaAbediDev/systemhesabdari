param([string]$TabId = 'tabCustomers')
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$found = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq $TabId) { $found = $e; break } }
if (-not $found) { Write-Output "TAB_NOT_FOUND $TabId"; exit 1 }
try {
    $tp = $found.GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern)
    Write-Output "BEFORE $TabId = $($tp.Current.ToggleState)"
    $tp.Toggle()
    Start-Sleep -Milliseconds 2000
    $found2 = $null
    $all2 = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
    foreach ($e in $all2) { if ($e.Current.AutomationId -eq $TabId) { $found2 = $e; break } }
    if ($found2) {
        $tp2 = $found2.GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern)
        Write-Output "AFTER $TabId = $($tp2.Current.ToggleState)"
    }
} catch { Write-Output "TOGGLE_FAIL $($_.Exception.Message)" }
