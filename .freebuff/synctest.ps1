param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Sync {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$tab = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'tabCustomers') { $tab = $e; break } }
if (-not $tab) { Write-Output "TAB_NOT_FOUND"; exit 1 }

$sync = $tab.GetCurrentPattern([System.Windows.Automation.SynchronizedInputPattern]::Pattern)
$eventFired = $false
$ev = Register-ObjectEvent -InputObject $tab -EventName InputReachedTargetEvent -Action { Set-Variable -Name eventFired -Value $true -Scope Script }
$sync.StartListening([System.Windows.Automation.SynchronizedInputType]::MouseLeftButtonDown)

$r = $tab.Current.BoundingRectangle
$x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
[void][Win32Sync]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 500
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
Start-Sleep -Milliseconds 300
[Win32Sync]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 150
[Win32Sync]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 3000

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("SYNC_EVENT_FIRED=$($script:eventFired)")
# toggle state after
$all2 = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
foreach ($e in $all2) {
    if ($e.Current.AutomationId -match '^tab') {
        $state = 'N/A'
        try { $tp = $e.GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern); $state = $tp.Current.ToggleState.ToString() } catch {}
        [void]$sb.AppendLine("TAB id=$($e.Current.AutomationId) toggle=$state")
    }
}
Unregister-Event -SourceIdentifier $ev.Name -ErrorAction SilentlyContinue
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\sm2_sync.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
