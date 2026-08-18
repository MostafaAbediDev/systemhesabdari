param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32AddB {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
}
"@
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$hwnd = $proc.MainWindowHandle
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)

function Get-All { $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition) }

function Set-Field($FieldId, $Value) {
    $all = Get-All
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
        Start-Sleep -Milliseconds 500
        Write-Output "SET id=$FieldId value='$($vp.Current.Value)'"
    } catch { Write-Output "SETVALUE_FAIL id=$FieldId $($_.Exception.Message)" }
}

Set-Field 'BankNameInput' 'ملت'
Set-Field 'CardNumberInput' '6219861975001234'
Set-Field 'AccountNumberInput' '123456789'
Set-Field 'ShabaInput' 'IR820170000000000000000001'

# click AddBankToTableButton
$all = Get-All
$btn = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'AddBankToTableButton') { $btn = $e; break } }
if ($btn) {
    $r = $btn.Current.BoundingRectangle
    if ($r.Width -gt 0 -and $r.Height -gt 0) {
        $x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
        [void][Win32AddB]::SetForegroundWindow($hwnd)
        Start-Sleep -Milliseconds 300
        [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
        Start-Sleep -Milliseconds 200
        [Win32AddB]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 80
        [Win32AddB]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
        Write-Output "CLICKED AddBankToTableButton at=$x,$y"
    }
}
Start-Sleep -Milliseconds 2000
Write-Output "DONE"
