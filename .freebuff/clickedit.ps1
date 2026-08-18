$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32CEd {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
function Get-Tree {
    $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
    return $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
}
$sb = New-Object System.Text.StringBuilder
# try candidates left of the name text, y = header center (~263)
$ys = 263
$xs = @(1335, 1343, 1351, 1359, 1367, 1375, 1383, 1391)
$opened = $false
foreach ($x in $xs) {
    [void][Win32CEd]::SetForegroundWindow($hwnd)
    Start-Sleep -Milliseconds 250
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $ys)
    Start-Sleep -Milliseconds 250
    [Win32CEd]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 80
    [Win32CEd]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
    [void]$sb.AppendLine("CLICK at=$x,$ys")
    Start-Sleep -Milliseconds 1500
    $all = Get-Tree
    $fn = 0
    foreach ($e in $all) { if ($e.Current.AutomationId -eq 'FirstNameInput') { $fn++ } }
    if ($fn -gt 0) { [void]$sb.AppendLine("EDIT_OPENED at x=$x"); $opened = $true; break }
}
if (-not $opened) { [void]$sb.AppendLine("EDIT_NOT_OPENED") }
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_clickedit.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
