param(
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\dlgcancel.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$sb = New-Object System.Text.StringBuilder

$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$btn = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'CancelButton') { $btn = $e; break } }
if (-not $btn) { [void]$sb.AppendLine("CANCEL_BUTTON_NOT_FOUND"); [IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NOT_FOUND"; exit 1 }
try {
    $btn.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke()
    [void]$sb.AppendLine("INVOKED CancelButton (بازگشت)")
} catch {
    [void]$sb.AppendLine("INVOKE_FAIL: $($_.Exception.Message)")
}
Start-Sleep -Milliseconds 1500
$proc.Refresh()

# dialog gone?
$rootEl2 = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all2 = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$dlgPresent = $false
$modalOpen = $false
$val = ''
foreach ($e in $all2) {
    if ($e.Current.Name -match 'تغییرات ذخیره‌نشده|خروج از فرم') { $dlgPresent = $true }
    if ($e.Current.AutomationId -eq 'SaveButton' -and $e.Current.IsOffscreen -eq $false) { $modalOpen = $true }
    if ($e.Current.AutomationId -eq 'PART_TextBox') {
        try {
            $v = $e.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern).Current.Value
            if ($v -and $e.Current.BoundingRectangle.X -gt 1400) { $val = $v }
        } catch {}
    }
}
[void]$sb.AppendLine("DIALOG_PRESENT=$dlgPresent")
[void]$sb.AppendLine("MODAL_STILL_OPEN=$modalOpen")
[void]$sb.AppendLine("FIRSTNAME_VALUE='$val'")
[void]$sb.AppendLine("alive=$(-not $proc.HasExited) responding=$($proc.Responding)")
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
