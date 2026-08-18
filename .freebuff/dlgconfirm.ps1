param(
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\dlgconfirm.txt'
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

# 1) ensure persons submenu open, invoke 'لیست اشخاص'
$all = Get-All $proc.MainWindowHandle
$subItem = $null
foreach ($e in $all) { if ($e.Current.Name -eq 'لیست اشخاص' -and $e.Current.ControlType.ProgrammaticName -match 'Button') { $subItem = $e; break } }
if (-not $subItem) {
    foreach ($e in $all) { if ($e.Current.AutomationId -eq 'BtnPersons') { try { $e.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke() } catch {} ; break } }
    Start-Sleep -Milliseconds 1500
    $all = Get-All $proc.MainWindowHandle
    foreach ($e in $all) { if ($e.Current.Name -eq 'لیست اشخاص' -and $e.Current.ControlType.ProgrammaticName -match 'Button') { $subItem = $e; break } }
}
if (-not $subItem) { [void]$sb.AppendLine("SUBMENU_NOT_FOUND"); [IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NOT_FOUND"; exit 1 }
try {
    $subItem.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke()
    [void]$sb.AppendLine("INVOKED 'لیست اشخاص'")
} catch { [void]$sb.AppendLine("INVOKE_FAIL1: $($_.Exception.Message)") }
Start-Sleep -Milliseconds 1500

# 2) find and confirm the warning dialog
$proc.Refresh()
$all2 = Get-All $proc.MainWindowHandle
$dlgPresent = $false
$confirmBtn = $null
foreach ($e in $all2) {
    if ($e.Current.Name -match 'تغییرات ذخیره‌نشده|خروج از فرم') { $dlgPresent = $true }
    if ($e.Current.AutomationId -eq 'ConfirmButton') { $confirmBtn = $e }
}
[void]$sb.AppendLine("DIALOG_PRESENT_BEFORE_CONFIRM=$dlgPresent")
if ($confirmBtn) {
    try {
        $confirmBtn.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke()
        [void]$sb.AppendLine("INVOKED ConfirmButton (خروج از فرم)")
    } catch { [void]$sb.AppendLine("INVOKE_FAIL2: $($_.Exception.Message)") }
}
Start-Sleep -Milliseconds 2500
$proc.Refresh()

# 3) verify: dialog gone, modal closed, navigation happened
$all3 = Get-All $proc.MainWindowHandle
$dlgAfter = $false
$modalOpen = $false
$listTitle = $false
foreach ($e in $all3) {
    if ($e.Current.Name -match 'تغییرات ذخیره‌نشده') { $dlgAfter = $true }
    if ($e.Current.AutomationId -eq 'SaveButton' -and $e.Current.IsOffscreen -eq $false) { $modalOpen = $true }
    if ($e.Current.AutomationId -eq 'PersonsGrid' -and $e.Current.IsOffscreen -eq $false) { $listTitle = $true }
}
[void]$sb.AppendLine("DIALOG_AFTER=$dlgAfter")
[void]$sb.AppendLine("MODAL_CLOSED=$(-not $modalOpen)")
[void]$sb.AppendLine("PERSON_LIST_VISIBLE=$listTitle")
[void]$sb.AppendLine("alive=$(-not $proc.HasExited) responding=$($proc.Responding)")
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
