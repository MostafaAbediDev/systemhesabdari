param([string]$Name = 'لیست اشخاص', [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\invokesub.txt')
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
foreach ($e in $all) {
    if ($e.Current.Name -eq $Name -and $e.Current.ControlType.ProgrammaticName -eq 'ControlType.Button' -and $e.Current.BoundingRectangle.Width -gt 0) { $btn = $e; break }
}
if (-not $btn) { [void]$sb.AppendLine("BTN_NOT_FOUND '$Name'"); [IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NOT_FOUND"; exit 1 }
try {
    $p = $btn.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
    $p.Invoke()
    [void]$sb.AppendLine("INVOKED '$Name' at rect=$($btn.Current.BoundingRectangle)")
} catch { [void]$sb.AppendLine("INVOKE_FAIL $($_.Exception.Message)") }
Start-Sleep -Milliseconds 2500
$proc.Refresh()
# check for dialog + modal state
$rootEl2 = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all2 = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$dlg = $false; $modal = $false
foreach ($e in $all2) {
    if ($e.Current.Name -match 'تغییرات ذخیره‌نشده|خروج از فرم|بازگشت') { $dlg = $true }
    if ($e.Current.AutomationId -eq 'SaveButton' -and -not $e.Current.IsOffscreen) { $modal = $true }
}
[void]$sb.AppendLine("DIALOG_PRESENT=$dlg")
[void]$sb.AppendLine("MODAL_OPEN=$modal")
[void]$sb.AppendLine("alive=$(-not $proc.HasExited) responding=$($proc.Responding)")
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
