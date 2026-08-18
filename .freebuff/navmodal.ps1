param(
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\navmodal.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$sb = New-Object System.Text.StringBuilder

# 1) ensure persons submenu is open (invoke BtnPersons if needed)
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$subItem = $null
foreach ($e in $all) {
    if ($e.Current.Name -eq 'لیست اشخاص' -and $e.Current.ControlType.ProgrammaticName -match 'Button') { $subItem = $e; break }
}
if (-not $subItem) {
    $btn = $null
    foreach ($e in $all) { if ($e.Current.AutomationId -eq 'BtnPersons') { $btn = $e; break } }
    if ($btn) {
        try { $btn.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke() } catch { [void]$sb.AppendLine("OPEN_SUBMENU_FAIL: $($_.Exception.Message)") }
        Start-Sleep -Milliseconds 1500
        $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
        $all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
        foreach ($e in $all) {
            if ($e.Current.Name -eq 'لیست اشخاص' -and $e.Current.ControlType.ProgrammaticName -match 'Button') { $subItem = $e; break }
        }
    }
}
if (-not $subItem) { [void]$sb.AppendLine("SUBMENU_ITEM_NOT_FOUND"); [IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NOT_FOUND"; exit 1 }

# 2) invoke the submenu item -> OnSubMenuClicked
try {
    $subItem.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke()
    [void]$sb.AppendLine("INVOKED submenu item 'لیست اشخاص'")
} catch {
    [void]$sb.AppendLine("INVOKE_FAIL: $($_.Exception.Message)")
}
Start-Sleep -Milliseconds 2000
$proc.Refresh()

# 3) check for extra windows (ModernDialog warning)
$rootEl2 = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
for ($i = 0; $i -lt $wins.Count; $i++) {
    $w = $wins.Item($i)
    [void]$sb.AppendLine("WIN[$i] title='$($w.Current.Name)' cls='$($w.Current.ClassName)'")
    # dump buttons of any non-main window (dialog)
    if ($w.Current.Name -notmatch 'تعادل|نرم') {
        $da = $w.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
        for ($j = 0; $j -lt $da.Count; $j++) {
            $de = $da.Item($j)
            if ($de.Current.ControlType.ProgrammaticName -match 'Button|Text' -and $de.Current.Name) {
                [void]$sb.AppendLine("   DIALOG[$j] $($de.Current.ControlType.ProgrammaticName) name='$($de.Current.Name)'")
            }
        }
    }
}

# 4) check main window tree for dialog text and modal state
$proc.Refresh()
$rootEl3 = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all3 = $rootEl3.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$dlgFound = $false
$modalOpen = $false
foreach ($e in $all3) {
    if ($e.Current.Name -match 'تغییرات ذخیره‌نشده|خروج از فرم|بازگشت') {
        [void]$sb.AppendLine("TREE: name='$($e.Current.Name)'")
        $dlgFound = $true
    }
    if ($e.Current.AutomationId -eq 'SaveButton' -and $e.Current.IsOffscreen -eq $false) { $modalOpen = $true }
}
[void]$sb.AppendLine("DIALOG_PRESENT=$dlgFound")
[void]$sb.AppendLine("MODAL_STILL_OPEN=$modalOpen")
[void]$sb.AppendLine("alive=$(-not $proc.HasExited) responding=$($proc.Responding)")
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
