$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32ClE {
  [DllImport("user32.dll")] public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
  [DllImport("user32.dll")] public static extern IntPtr FindWindow(string cls, string title);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$rootEl = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
$wins = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
$closed = 0
for ($i = 0; $i -lt $wins.Count; $i++) {
    $w = $wins.Item($i)
    if ($w.Current.Name -eq 'خطا' -or $w.Current.Name -match 'خطا') {
        $hw = New-Object System.IntPtr($w.Current.NativeWindowHandle)
        [void][Win32ClE]::PostMessage($hw, 0x0010, [IntPtr]::Zero, [IntPtr]::Zero)
        Write-Output "WM_CLOSE sent to error window hwnd=$hw"
        $closed++
    }
}
Write-Output "CLOSED=$closed"
Start-Sleep -Milliseconds 1500
