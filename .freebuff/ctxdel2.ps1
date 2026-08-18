$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Cd2 {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\ctxdel2_result.txt'
$sb = New-Object System.Text.StringBuilder

$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$hwnd = $proc.MainWindowHandle

# 1) right-click on row1 name text (find Text named 'اریاگستر')
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$target = $null
foreach ($e in $all) {
    if ($e.Current.Name -eq 'اریاگستر' -and $e.Current.ControlType.ProgrammaticName -match 'Text') { $target = $e; break }
}
if (-not $target) { [void]$sb.AppendLine("ROW_TEXT_NOT_FOUND"); [IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true))); Write-Output "ROW_TEXT_NOT_FOUND"; exit 1 }
$r = $target.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32Cd2]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 200
[Win32Cd2]::mouse_event(0x0008, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 80
[Win32Cd2]::mouse_event(0x0010, 0, 0, 0, [UIntPtr]::Zero)
[void]$sb.AppendLine("RCLICK on row text at=$x,$y")
Start-Sleep -Milliseconds 2000

# 2) find context-menu Delete (name='حذف' AND id != 'BtnText'), click it
$rootEl2 = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all2 = $rootEl2.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$del = $null
foreach ($e in $all2) {
    if ($e.Current.Name -eq 'حذف' -and $e.Current.AutomationId -ne 'BtnText') { $del = $e; break }
}
if (-not $del) {
    [void]$sb.AppendLine("CTX_DELETE_NOT_FOUND (menu may not be open)")
} else {
    $r2 = $del.Current.BoundingRectangle
    $x2 = [int]($r2.X + $r2.Width/2); $y2 = [int]($r2.Y + $r2.Height/2)
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x2, $y2)
    Start-Sleep -Milliseconds 200
    [Win32Cd2]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 60
    [Win32Cd2]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
    [void]$sb.AppendLine("CLICKED context-menu حذف at=$x2,$y2")
}
Start-Sleep -Milliseconds 2500
$proc.Refresh()
[void]$sb.AppendLine("after: alive=$(-not $proc.HasExited) responding=$($proc.Responding) memMB=$([math]::Round($proc.WorkingSet64/1MB))")

# 3) window list (any dialog?)
$rootEl3 = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl3.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
for ($i = 0; $i -lt $wins.Count; $i++) {
    $w = $wins.Item($i)
    [void]$sb.AppendLine("WIN[$i] title='$($w.Current.Name)' type=$($w.Current.ControlType.ProgrammaticName)")
}
# 4) main window tree: any confirmation dialog text?
$rootEl4 = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all4 = $rootEl4.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$hits = @()
foreach ($e in $all4) {
    $n = $e.Current.Name
    if ($n -and ($n -match 'مطمئن|حذف|تأیید|تایید|بله|خیر|انصراف')) { $hits += "$($e.Current.ControlType.ProgrammaticName):'$n':id=$($e.Current.AutomationId):en=$($e.Current.IsEnabled):off=$($e.Current.IsOffscreen)" }
}
if ($hits.Count -eq 0) { [void]$sb.AppendLine("NO_CONFIRM_TEXT") } else { [void]$sb.AppendLine("CONFIRM_TEXT: $($hits -join ' | ')") }
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
