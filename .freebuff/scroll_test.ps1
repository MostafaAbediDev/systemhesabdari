param([int]$Pct = 100)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$p = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $p) { Write-Output "NO_PROC"; exit }
$h = $p.MainWindowHandle
if ($h -eq 0) { Write-Output "NO_HWND"; exit }
$root = [System.Windows.Automation.AutomationElement]::FromHandle($h)
$all = $root.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
for ($i = 0; $i -lt $all.Count; $i++) {
    $e = $all.Item($i)
    if ($e.Current.ControlType.ProgrammaticName -eq 'ControlType.DataGrid') {
        try {
            $sp = $e.GetCurrentPattern([System.Windows.Automation.ScrollPattern]::Pattern)
            $hsp = [math]::Round($sp.Current.HorizontalScrollPercent)
            $hvs = [math]::Round($sp.Current.HorizontalViewSize, 1)
            $vvs = [math]::Round($sp.Current.VerticalViewSize, 1)
            Write-Output ("DataGrid idx=$i HScroll=$hsp HView=$hvs VView=$vvs")
            if ($Pct -ge 0) {
                $sp.SetScrollPercent([double]$Pct, [System.Windows.Automation.ScrollPattern]::NoScroll)
                Write-Output ("SCROLLED_TO $Pct")
            }
        } catch {
            Write-Output ("NOPATTERN idx=$i err=" + $_.Exception.Message)
        }
    }
}
