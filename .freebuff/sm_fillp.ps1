param([string]$Name = 'QA', [string]$Last = 'Bulk1', [string]$NCode = '0499370899')
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32FillP {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)

function Set-Field($FieldId, $Value) {
    $outer = $null
    foreach ($e in $all) { if ($e.Current.AutomationId -eq $FieldId) { $outer = $e; break } }
    if (-not $outer) { Write-Output "OUTER_NOT_FOUND id=$FieldId"; return }
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
    if (-not $inner) { Write-Output "INNER_NOT_FOUND id=$FieldId"; return }
    try {
        $vp = $inner.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        $vp.SetValue($Value)
        Start-Sleep -Milliseconds 600
        Write-Output "SET id=$FieldId value='$($vp.Current.Value)'"
    } catch {
        Write-Output "SETVALUE_FAIL id=$FieldId $($_.Exception.Message)"
    }
}

Set-Field 'FirstNameInput' $Name
Set-Field 'LastNameInput' $Last
Set-Field 'NationalCodeInput' $NCode

# move focus to MobileInput to push bindings (dirty flag)
$mobile = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'MobileInput') { $mobile = $e; break } }
if ($mobile) {
    $r = $mobile.Current.BoundingRectangle
    if ($r.Width -gt 0 -and $r.Height -gt 0) {
        $x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
        [void][Win32FillP]::SetForegroundWindow($hwnd)
        Start-Sleep -Milliseconds 300
        [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
        Start-Sleep -Milliseconds 150
        [Win32FillP]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 60
        [Win32FillP]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
        Write-Output "FOCUS_MOVED to MobileInput"
    }
}
Start-Sleep -Milliseconds 1200
Write-Output "DONE"
