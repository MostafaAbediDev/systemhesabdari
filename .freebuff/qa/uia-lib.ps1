# Shared UIAutomation helpers for Taadol QA
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms

Add-Type @"
using System;
using System.Runtime.InteropServices;
public static class MouseWin32 {
    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);
    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);
    public const uint LEFTDOWN = 0x02;
    public const uint LEFTUP = 0x04;
    public const uint RIGHTDOWN = 0x08;
    public const uint RIGHTUP = 0x10;
    public static void LeftClick(int x, int y) {
        SetCursorPos(x, y);
        System.Threading.Thread.Sleep(60);
        mouse_event(LEFTDOWN, 0, 0, 0, IntPtr.Zero);
        System.Threading.Thread.Sleep(40);
        mouse_event(LEFTUP, 0, 0, 0, IntPtr.Zero);
    }
    public static void RightClick(int x, int y) {
        SetCursorPos(x, y);
        System.Threading.Thread.Sleep(60);
        mouse_event(RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
        System.Threading.Thread.Sleep(40);
        mouse_event(RIGHTUP, 0, 0, 0, IntPtr.Zero);
    }
    public static void MoveTo(int x, int y) {
        SetCursorPos(x, y);
        System.Threading.Thread.Sleep(80);
    }
}
"@

Add-Type @"
using System;
using System.Runtime.InteropServices;
public static class KeyWin32 {
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    public const byte VK_ESCAPE = 0x1B;
    public const byte VK_RETURN = 0x0D;
    public const byte VK_TAB = 0x09;
    public static void SendKey(byte vk) {
        keybd_event(vk, 0, 0, UIntPtr.Zero);
        keybd_event(vk, 0, 2, UIntPtr.Zero);
        System.Threading.Thread.Sleep(60);
    }
}
"@

$script:TaadolPid = $env:TAADOL_PID
if (-not $script:TaadolPid) { $script:TaadolPid = 19208 }

function Get-TaadolWindow {
    $root = [System.Windows.Automation.AutomationElement]::RootElement
    $cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, [int]$script:TaadolPid)
    return $root.FindFirst([System.Windows.Automation.TreeScope]::Children, $cond)
}

function Find-El($win, $prop, $value) {
    $cond = New-Object System.Windows.Automation.PropertyCondition($prop, $value)
    return $win.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $cond)
}

function Find-Els($win, $prop, $value) {
    $cond = New-Object System.Windows.Automation.PropertyCondition($prop, $value)
    return $win.FindAll([System.Windows.Automation.TreeScope]::Descendants, $cond)
}

function Find-ByName($win, $name) {
    return Find-El $win ([System.Windows.Automation.AutomationElement]::NameProperty) $name
}

function Find-ByAId($win, $aid) {
    return Find-El $win ([System.Windows.Automation.AutomationElement]::AutomationIdProperty) $aid
}

function Click-El($el) {
    if (-not $el) { Write-Output "CLICK: element not found"; return $false }
    try {
        $invoke = $el.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
        $invoke.Invoke()
        Write-Output ("CLICK(invoke): " + $el.Current.Name + " / " + $el.Current.AutomationId)
        return $true
    } catch {
        $r = $el.Current.BoundingRectangle
        if ($r.Width -gt 0 -and $r.Height -gt 0) {
            $x = [int]($r.X + $r.Width / 2)
            $y = [int]($r.Y + $r.Height / 2)
            [MouseWin32]::LeftClick($x, $y)
            Write-Output ("CLICK(mouse): " + $el.Current.Name + " / " + $el.Current.AutomationId + " @ " + $x + "," + $y)
            return $true
        }
        Write-Output "CLICK: no invoke pattern and no rect"
        return $false
    }
}

function RightClick-El($el) {
    if (-not $el) { Write-Output "RCLICK: element not found"; return $false }
    $r = $el.Current.BoundingRectangle
    if ($r.Width -gt 0 -and $r.Height -gt 0) {
        $x = [int]($r.X + $r.Width / 2)
        $y = [int]($r.Y + $r.Height / 2)
        [MouseWin32]::RightClick($x, $y)
        Write-Output ("RCLICK @ " + $x + "," + $y)
        return $true
    }
    Write-Output "RCLICK: no rect"
    return $false
}

function Set-Text($el, $text) {
    if (-not $el) { Write-Output "SETTEXT: element not found"; return $false }
    try {
        $vp = $el.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        $vp.SetValue($text)
        Write-Output ("SETTEXT: '" + $text + "'")
        return $true
    } catch {
        Write-Output ("SETTEXT FAILED: " + $_.Exception.Message)
        return $false
    }
}

function Get-Text($el) {
    if (-not $el) { return "" }
    try {
        return $el.Current.Name
    } catch { return "" }
}

function Dump-TreeToFile($el, $path, $maxDepth) {
    "" | Out-File -FilePath $path -Encoding utf8
    function Dump-Tree($e, $depth, $md) {
        if ($depth -gt $md) { return }
        $name = $e.Current.Name
        $ct = $e.Current.ControlType.ProgrammaticName -replace 'ControlType\.', ''
        $aid = $e.Current.AutomationId
        $line = ("{0}{1} | {2} | Name=[{3}] | AId=[{4}]" -f ('  ' * $depth), $ct, $depth, $name, $aid)
        Add-Content -Path $path -Value $line -Encoding utf8
        if ($depth -ge $md) { return }
        $children = $e.FindAll([System.Windows.Automation.TreeScope]::Children, [System.Windows.Automation.Condition]::TrueCondition)
        foreach ($c in $children) { Dump-Tree $c ($depth + 1) $md }
    }
    Dump-Tree $el 0 $maxDepth
}

function Wait-Ms($ms) { Start-Sleep -Milliseconds $ms }

function Set-Foreground($win) {
    $hwnd = New-Object System.IntPtr($win.Current.NativeWindowHandle)
    [KeyWin32]::SetForegroundWindow($hwnd)
}
