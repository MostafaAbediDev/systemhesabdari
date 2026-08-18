param(
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\navtest.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Nav {
  [DllImport("user32.dll")]
  public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")]
  public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@

$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)

$btn = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'BtnDashboard') { $btn = $e; break } }
if (-not $btn) { Write-Output "BTN_DASHBOARD_NOT_FOUND"; exit 1 }
$r = $btn.Current.BoundingRectangle
if ($r.Width -le 0) { Write-Output "NO_RECT id=BtnDashboard off=$($btn.Current.IsOffscreen)"; exit 1 }
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)

[void][Win32Nav]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 200
[Win32Nav]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32Nav]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("CLICK BtnDashboard at=$x,$y rect=$($r.X),$($r.Y),$($r.Width),$($r.Height)")
Start-Sleep -Milliseconds 1800

# 1) check for extra windows (ModernDialog)
$rootEl2 = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
for ($i = 0; $i -lt $wins.Count; $i++) {
    $w = $wins.Item($i)
    [void]$sb.AppendLine("WIN[$i] title='$($w.Current.Name)' cls='$($w.Current.ClassName)'")
}

# 2) check main window tree for dialog texts
$proc.Refresh()
$rootEl3 = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all3 = $rootEl3.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$foundDlg = $false
foreach ($e in $all3) {
    if ($e.Current.Name -match 'تغییرات ذخیره‌نشده|خروج از فرم|بازگشت|ذخیره شخص') {
        [void]$sb.AppendLine("TREE name='$($e.Current.Name)' id='$($e.Current.AutomationId)'")
        if ($e.Current.Name -match 'تغییرات ذخیره‌نشده|خروج از فرم') { $foundDlg = $true }
    }
}
[void]$sb.AppendLine("DIALOG_PRESENT=$foundDlg")
[void]$sb.AppendLine("alive=$(-not $proc.HasExited) responding=$($proc.Responding) memMB=$([math]::Round($proc.WorkingSet64/1MB))")
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
