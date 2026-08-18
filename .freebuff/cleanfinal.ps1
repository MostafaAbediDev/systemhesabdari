param(
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\cleanfinal.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Cf {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$sb = New-Object System.Text.StringBuilder

function Get-All($h) {
    return ([System.Windows.Automation.AutomationElement]::FromHandle($h)).FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
}

# 1) open modal via mouse click on BtnNewText
$all = Get-All $proc.MainWindowHandle
$btnNew = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'BtnNewText') { $btnNew = $e; break } }
if (-not $btnNew) { [void]$sb.AppendLine("BtnNewText_NOT_FOUND"); [IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "NOT_FOUND"; exit 1 }
$r = $btnNew.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32Cf]::SetForegroundWindow($proc.MainWindowHandle)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 200
[Win32Cf]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32Cf]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
[void]$sb.AppendLine("CLICKED BtnNewText at=$x,$y")
Start-Sleep -Milliseconds 1500
$proc.Refresh()
$all = Get-All $proc.MainWindowHandle
$modalOpen = $false
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'SaveButton' -and -not $e.Current.IsOffscreen) { $modalOpen = $true } }
[void]$sb.AppendLine("MODAL_OPEN_AFTER_CLICK=$modalOpen")
if (-not $modalOpen) { [IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "MODAL_NOT_OPEN"; exit 1 }

# 2) navigate via 'لیست اشخاص' (InvokePattern on submenu item)
$subItem = $null
foreach ($e in $all) { if ($e.Current.Name -eq 'لیست اشخاص' -and $e.Current.ControlType.ProgrammaticName -match 'Button') { $subItem = $e; break } }
if (-not $subItem) {
    foreach ($e in $all) { if ($e.Current.AutomationId -eq 'BtnPersons') { try { $e.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke() } catch {} ; break } }
    Start-Sleep -Milliseconds 1500
    $all = Get-All $proc.MainWindowHandle
    foreach ($e in $all) { if ($e.Current.Name -eq 'لیست اشخاص' -and $e.Current.ControlType.ProgrammaticName -match 'Button') { $subItem = $e; break } }
}
if ($subItem) {
    try { $subItem.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke(); [void]$sb.AppendLine("INVOKED 'لیست اشخاص' (clean modal)") } catch { [void]$sb.AppendLine("NAV_FAIL: $($_.Exception.Message)") }
} else { [void]$sb.AppendLine("SUBMENU_NOT_FOUND") }
Start-Sleep -Milliseconds 2500
$proc.Refresh()

# 3) verify
$all3 = Get-All $proc.MainWindowHandle
$dlg = $false
$modalStill = $false
$listVis = $false
foreach ($e in $all3) {
    if ($e.Current.Name -match 'تغییرات ذخیره‌نشده|خروج از فرم') { $dlg = $true }
    if ($e.Current.AutomationId -eq 'SaveButton' -and $e.Current.IsOffscreen -eq $false) { $modalStill = $true }
    if ($e.Current.AutomationId -eq 'PersonsGrid' -and $e.Current.IsOffscreen -eq $false) { $listVis = $true }
}
[void]$sb.AppendLine("DIALOG_PRESENT=$dlg")
[void]$sb.AppendLine("MODAL_CLOSED=$(-not $modalStill)")
[void]$sb.AppendLine("PERSON_LIST_VISIBLE=$listVis")
[void]$sb.AppendLine("alive=$(-not $proc.HasExited) responding=$($proc.Responding)")
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
