param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$sb = New-Object System.Text.StringBuilder
$ids = @('FirstNameInput','LastNameInput','NationalCodeInput','PhoneInput','MobileInput','EmailInput','CreditLimitInput','CodeInput')
foreach ($FieldId in $ids) {
    $outer = $null
    foreach ($e in $all) { if ($e.Current.AutomationId -eq $FieldId) { $outer = $e; break } }
    if (-not $outer) { [void]$sb.AppendLine("$FieldId = NOT_PRESENT"); continue }
    $inner = $null
    foreach ($e in $all) {
        if ($e.Current.AutomationId -eq 'PART_TextBox') {
            $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
            $node = $e
            while ($node -ne $null) {
                if ($node -eq $outer) { $inner = $e; break }
                $node = $walker.GetParent($node)
            }
            if ($inner) { break }
        }
    }
    if (-not $inner) { [void]$sb.AppendLine("$FieldId = NO_INNER"); continue }
    try {
        $vp = $inner.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        [void]$sb.AppendLine("$FieldId = [$($vp.Current.Value)]")
    } catch { [void]$sb.AppendLine("$FieldId = NO_VALUE_PATTERN") }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\sm_fieldvals.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "SAVED"
