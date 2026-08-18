param([string]$Value = 'QA')
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$outer = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'FirstNameInput') { $outer = $e; break } }
if (-not $outer) { Write-Output "OUTER_NOT_FOUND"; exit 1 }
$inner = $null
foreach ($e in $all) {
    if ($e.Current.AutomationId -eq 'PART_TextBox') {
        # must be descendant of outer
        try {
            $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
            $node = $e
            while ($node -ne $null) {
                if ($node -eq $outer) { $inner = $e; break }
                $node = $walker.GetParent($node)
            }
        } catch {}
        if ($inner) { break }
    }
}
if (-not $inner) { Write-Output "INNER_NOT_FOUND"; exit 1 }
try {
    $vp = $inner.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    $vp.SetValue($Value)
    Start-Sleep -Milliseconds 800
    $read = $vp.Current.Value
    Write-Output "SET value='$read' ok=$($read -eq $Value)"
} catch {
    Write-Output "SETVALUE_FAIL $($_.Exception.Message)"
}
