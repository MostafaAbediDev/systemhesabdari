param(
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\cleanfinal2.txt'
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

$all = Get-All $proc.MainWindowHandle
$subItem = $null
foreach ($e in $all) { if ($e.Current.Name -eq 'شخص جدید' -and $e.Current.ControlType.ProgrammaticName -match 'Button') { $subItem = $e; break } }
if (-not $subItem) {
    foreach ($e in $all) { if ($e.Current.AutomationId -eq 'BtnPersons') { try { $e.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke() } catch {} ; break } }
    Start-Sleep -Milliseconds 1500
    $all = Get-All $proc.MainWindowHandle
    foreach ($e in $all) { if ($e.Current.Name -eq 'شخص جدید' -and $e.Current.ControlType.ProgrammaticName -match 'Button') { $subItem = $e; break } }
}
if ($subItem) {
    try { $subItem.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke(); [void]$sb.AppendLine("INVOKED 'شخص جدید' (clean modal)") } catch { [void]$sb.AppendLine("NAV_FAIL: $($_.Exception.Message)") }
} else { [void]$sb.AppendLine("SUBMENU_NOT_FOUND") }
Start-Sleep -Milliseconds 2500
$proc.Refresh()

$all3 = Get-All $proc.MainWindowHandle
$dlg = $false
$modalStill = $false
$personNew = $false
foreach ($e in $all3) {
    if ($e.Current.Name -match 'تغییرات ذخیره‌نشده|خروج از فرم') { $dlg = $true }
    if ($e.Current.AutomationId -eq 'SaveButton' -and $e.Current.IsOffscreen -eq $false) { $modalStill = $true }
    if ($e.Current.AutomationId -eq 'FirstNameInput' -and $e.Current.IsOffscreen -eq $false) { $personNew = $true }
}
[void]$sb.AppendLine("DIALOG_PRESENT=$dlg")
[void]$sb.AppendLine("MODAL_CLOSED=$(-not $modalStill)")
[void]$sb.AppendLine("PERSON_NEW_VIEW_VISIBLE=$personNew")
[void]$sb.AppendLine("alive=$(-not $proc.HasExited) responding=$($proc.Responding)")
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
