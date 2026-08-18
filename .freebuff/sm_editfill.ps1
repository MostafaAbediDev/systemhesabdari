param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32EdFill {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)

$outer = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'LastNameInput') { $outer = $e; break } }
if (-not $outer) { Write-Output "OUTER_NOT_FOUND"; exit 1 }
$inner = $null
foreach ($e in $all) {
    if ($e.Current.AutomationId -eq 'PART_TextBox') {
        $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
        $node = $e
        while ($node -ne $null) {
            if ($node -eq $outer) { $inner = $e; break }
            $node = $walker.GetParent($node)
        }
        if ($inner) { break }
    }
}
if (-not $inner) { Write-Output "INNER_NOT_FOUND"; exit 1 }
try {
    $vp = $inner.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    $vp.SetValue('FinalTest2')
    Start-Sleep -Milliseconds 800
    Write-Output "SET LastNameInput='$($vp.Current.Value)'"
} catch { Write-Output "SETVALUE_FAIL $($_.Exception.Message)"; exit 1 }

# move focus to NationalCodeInput to push binding
$nc = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'NationalCodeInput') { $nc = $e; break } }
if ($nc) {
    $r = $nc.Current.BoundingRectangle
    if ($r.Width -gt 0 -and $r.Height -gt 0) {
        $x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
        [void][Win32EdFill]::SetForegroundWindow($hwnd)
        Start-Sleep -Milliseconds 300
        [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
        Start-Sleep -Milliseconds 150
        [Win32EdFill]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 60
        [Win32EdFill]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
        Write-Output "FOCUS_MOVED to NationalCodeInput"
    }
}
Start-Sleep -Milliseconds 1200
Write-Output "DONE"
