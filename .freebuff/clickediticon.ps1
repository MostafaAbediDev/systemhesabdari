param([int]$X = 1509, [int]$Y = 263)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32CEI {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
[void][Win32CEI]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 400
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($X, $Y)
Start-Sleep -Milliseconds 300
[Win32CEI]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 100
[Win32CEI]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 3500
$proc.Refresh()
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("CLICKED at=$X,$Y alive=$(-not $proc.HasExited) responding=$($proc.Responding)")
$fn = 0; $err = 0; $sv = 0
foreach ($e in $all) {
    if ($e.Current.AutomationId -eq 'FirstNameInput') { $fn++ }
    if ($e.Current.Name -like 'ERROR:*') { $err++ }
    if ($e.Current.AutomationId -eq 'SaveButton') { $sv++ }
}
[void]$sb.AppendLine("FirstNameInput=$fn SaveButton=$sv ERROR=$err")
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_editclick.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> fx_editclick.txt"
