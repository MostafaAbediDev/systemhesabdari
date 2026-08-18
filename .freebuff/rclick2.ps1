param(
    [string]$Name = 'اریاگستر',
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\rclick2_result.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Rc2 {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
  [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
  [StructLayout(LayoutKind.Sequential)]
  public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$found = $null
foreach ($e in $all) {
    if ($e.Current.Name -eq $Name -and $e.Current.ControlType.ProgrammaticName -match 'Text') { $found = $e; break }
}
if (-not $found) { Write-Output "NOT_FOUND name='$Name'"; exit 1 }
$r = $found.Current.BoundingRectangle
if ($r.Width -le 0 -or $r.Height -le 0) { Write-Output "NO_RECT off=$($found.Current.IsOffscreen)"; exit 1 }
[void][Win32Rc2]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 200
[Win32Rc2]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32Rc2]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("RCLICK name='$Name' at=$x,$y")
Start-Sleep -Milliseconds 2500
$proc.Refresh()
[void]$sb.AppendLine("after: alive=$(-not $proc.HasExited) responding=$($proc.Responding) memMB=$([math]::Round($proc.WorkingSet64/1MB))")
$rootEl2 = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
for ($i = 0; $i -lt $wins.Count; $i++) {
    $w = $wins.Item($i)
    [void]$sb.AppendLine("WIN[$i] title='$($w.Current.Name)'")
}
# context menu items visible?
$rootEl3 = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all3 = $rootEl3.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$ctx = 0
foreach ($e in $all3) { if ($e.Current.Name -eq 'ویرایش' -or $e.Current.Name -eq 'حذف') { $ctx++ } }
[void]$sb.AppendLine("CTXMENU_HITS=$ctx")
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "RCLICK_DONE -> $Out"
