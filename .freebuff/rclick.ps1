param(
    [int]$Idx = 57,
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\rclick_result.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Rc {
  [DllImport("user32.dll")]
  public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")]
  public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
  [DllImport("user32.dll")]
  public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
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
if ($Idx -ge $all.Count) { Write-Output "BAD_IDX $Idx count=$($all.Count)"; exit 1 }
$found = $all.Item($Idx)
$r = $found.Current.BoundingRectangle
if ($r.Width -le 0 -or $r.Height -le 0) { Write-Output "NO_RECT idx=$Idx off=$($found.Current.IsOffscreen)"; exit 1 }
[void][Win32Rc]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 200
# real right-button down/up via mouse_event
[Win32Rc]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32Rc]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("RCLICK idx=$Idx name='$($found.Current.Name)' id='$($found.Current.AutomationId)' at=$x,$y off=$($found.Current.IsOffscreen)")
Start-Sleep -Milliseconds 2500
$proc.Refresh()
[void]$sb.AppendLine("after: alive=$(-not $proc.HasExited) responding=$($proc.Responding) memMB=$([math]::Round($proc.WorkingSet64/1MB))")
# dump window titles to detect the error dialog
$rootEl2 = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
for ($i = 0; $i -lt $wins.Count; $i++) {
    $w = $wins.Item($i)
    [void]$sb.AppendLine("WIN[$i] title='$($w.Current.Name)'")
}
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "RCLICK_DONE -> $Out"
