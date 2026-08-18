param(
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\cleanmodal.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$sb = New-Object System.Text.StringBuilder

function Get-All($h) {
    return ([System.Windows.Automation.AutomationElement]::FromHandle($h)).FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
}

# 1) open New Person modal via BtnNewText (InvokePattern)
$all = Get-All $proc.MainWindowHandle
$btnNew = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'BtnNewText') { $btnNew = $e; break } }
if (-not $btnNew) { [void]$sb.AppendLine("BtnNewText_NOT_FOUND"); [IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NOT_FOUND"; exit 1 }
try {
    $btnNew.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke()
    [void]$sb.AppendLine("OPENED New Person modal")
} catch { [void]$sb.AppendLine("OPEN_FAIL: $($_.Exception.Message)") }
Start-Sleep -Milliseconds 1200

# 2) navigate via 'لیست اشخاص' submenu
$all2 = Get-All $proc.MainWindowHandle
$subItem = $null
foreach ($e in $all2) { if ($e.Current.Name -eq 'لیست اشخاص' -and $e.Current.ControlType.ProgrammaticName -match 'Button') { $subItem = $e; break } }
if (-not $subItem) {
    foreach ($e in $all2) { if ($e.Current.AutomationId -eq 'BtnPersons') { try { $e.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke() } catch {} ; break } }
    Start-Sleep -Milliseconds 1500
    $all2 = Get-All $proc.MainWindowHandle
    foreach ($e in $all2) { if ($e.Current.Name -eq 'لیست اشخاص' -and $e.Current.ControlType.ProgrammaticName -match 'Button') { $subItem = $e; break } }
}
if ($subItem) {
    try { $subItem.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke(); [void]$sb.AppendLine("INVOKED 'لیست اشخاص' (clean modal)") } catch { [void]$sb.AppendLine("NAV_FAIL: $($_.Exception.Message)") }
} else { [void]$sb.AppendLine("SUBMENU_NOT_FOUND") }
Start-Sleep -Milliseconds 2500
$proc.Refresh()

# 3) verify
$all3 = Get-All $proc.MainWindowHandle
$dlg = $false
$modalOpen = $false
$listVis = $false
foreach ($e in $all3) {
    if ($e.Current.Name -match 'تغییرات ذخیره‌نشده|خروج از فرم') { $dlg = $true }
    if ($e.Current.AutomationId -eq 'SaveButton' -and $e.Current.IsOffscreen -eq $false) { $modalOpen = $true }
    if ($e.Current.AutomationId -eq 'PersonsGrid' -and $e.Current.IsOffscreen -eq $false) { $listVis = $true }
}
[void]$sb.AppendLine("DIALOG_PRESENT=$dlg")
[void]$sb.AppendLine("MODAL_CLOSED=$(-not $modalOpen)")
[void]$sb.AppendLine("PERSON_LIST_VISIBLE=$listVis")
[void]$sb.AppendLine("alive=$(-not $proc.HasExited) responding=$($proc.Responding)")
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
