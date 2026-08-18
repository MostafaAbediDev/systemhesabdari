param(
    [string]$Cmd = 'snapshot',
    [string]$Name = '',
    [string]$Marker = '',
    [string]$Out = '',
    [int]$W = 0,
    [int]$H = 0,
    [string]$ProcId = ''
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Qa {
  [DllImport("user32.dll", SetLastError=true)]
  public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
  [DllImport("user32.dll")]
  public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
  [DllImport("user32.dll")]
  public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
  [DllImport("user32.dll")]
  public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")]
  public static extern IntPtr GetForegroundWindow();
  [DllImport("user32.dll")]
  public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
  [DllImport("user32.dll")]
  public static extern bool IsWindowVisible(IntPtr hWnd);
  [StructLayout(LayoutKind.Sequential)]
  public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
}
"@

$exe = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\src\FrontEndWPF\Taadol\Taadol\bin\Debug\net9.0-windows\Taadol.exe'
$root = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff'

function Get-Proc {
    if ($ProcId) { Get-Process -Id ([int]$ProcId) -ErrorAction SilentlyContinue }
    else { Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1 }
}

function Write-Snapshot([string]$file, $hwnd) {
    $sb = New-Object System.Text.StringBuilder
    if ($hwnd -eq 0) { [void]$sb.AppendLine("NO WINDOW"); }
    else {
        $rect = New-Object Win32Qa+RECT
        [void][Win32Qa]::GetWindowRect($hwnd, [ref]$rect)
        [void]$sb.AppendLine("WINDOW size=$($rect.Right-$rect.Left)x$($rect.Bottom-$rect.Top)")
        $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
        $all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
        [void]$sb.AppendLine("ELEMENTS=$($all.Count)")
        for ($i=0; $i -lt $all.Count; $i++) {
            $e = $all.Item($i)
            $t = $e.Current.ControlType.ProgrammaticName
            $n = $e.Current.Name
            $id = $e.Current.AutomationId
            $en = $e.Current.IsEnabled
            $off = $e.Current.IsOffscreen
            [void]$sb.AppendLine("[$i] $t | name='$n' | id='$id' | en=$en | off=$off")
        }
    }
    [IO.File]::WriteAllText($file, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
}

function Get-Hwnd($proc) {
    $proc.Refresh()
    return $proc.MainWindowHandle
}

function Invoke-ByName($hwnd, [string]$target, [string]$outFile) {
    $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
    $all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
    $found = $null
    for ($i=0; $i -lt $all.Count; $i++) {
        $e = $all.Item($i)
        if ($e.Current.Name -eq $target) { $found = $e; break }
    }
    if (-not $found) {
        for ($i=0; $i -lt $all.Count; $i++) {
            $e = $all.Item($i)
            if ($e.Current.Name -and $e.Current.Name.Contains($target)) { $found = $e; break }
        }
    }
    if (-not $found) {
        for ($i=0; $i -lt $all.Count; $i++) {
            $e = $all.Item($i)
            if ($e.Current.AutomationId -eq $target) { $found = $e; break }
        }
    }
    if (-not $found) {
        Write-Output "NOT_FOUND target='$target'"
        return
    }
    $invokable = $null
    $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
    $node = $found
    while ($node -ne $null) {
        try {
            $p = $node.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
            $invokable = $node
            break
        } catch {}
        $node = $walker.GetParent($node)
    }
    if ($invokable) {
        Write-Output "INVOKED name='$($invokable.Current.Name)' id='$($invokable.Current.AutomationId)' type=$($invokable.Current.ControlType.ProgrammaticName) enabled=$($invokable.Current.IsEnabled) offscreen=$($invokable.Current.IsOffscreen)"
        try {
            $p2i = $invokable.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
            $p2i.Invoke(); Write-Output "INVOKE_OK"
        } catch { Write-Output "INVOKE_FAIL $($_.Exception.Message)" }
    } else {
        # try SelectionItem / ExpandCollapse as fallback
        try {
            $sel = $found.GetCurrentPattern([System.Windows.Automation.SelectionItemPattern]::Pattern)
            $sel.Select(); Write-Output "SELECTED name='$($found.Current.Name)'"
        } catch {
            try {
                $exp = $found.GetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern)
                $exp.Expand(); Write-Output "EXPANDED name='$($found.Current.Name)'"
            } catch { Write-Output "NO_PATTERN name='$($found.Current.Name)'" }
        }
    }
    Start-Sleep -Milliseconds 1800
    if ($outFile) {
        $p2 = Get-Proc
        if ($p2) { Write-Snapshot $outFile (Get-Hwnd $p2) }
    }
}

$proc = Get-Proc
if (-not $proc) {
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    $proc = Start-Process -FilePath $exe -PassThru
    $deadline = (Get-Date).AddSeconds(45)
    do {
        Start-Sleep -Milliseconds 300
        $proc.Refresh()
    } while ($proc.MainWindowHandle -eq 0 -and -not $proc.HasExited -and (Get-Date) -lt $deadline)
    $sw.Stop()
    Write-Output "LAUNCHED pid=$($proc.Id) startupMs=$($sw.ElapsedMilliseconds)"
} else {
    Write-Output "ALREADY_RUNNING pid=$($proc.Id)"
}

switch ($Cmd) {
    'snapshot' {
        $p2 = Get-Proc
        $p2.Refresh()
        Write-Output "pid=$($p2.Id) title='$($p2.MainWindowTitle)' responding=$($p2.Responding) memMB=$([math]::Round($p2.WorkingSet64/1MB)) exited=$($p2.HasExited)"
        if (-not $Out) { $Out = "$root\uia_snapshot.txt" }
        Write-Snapshot $Out (Get-Hwnd $p2)
        Write-Output "SNAPSHOT_SAVED $Out"
    }
    'buttons' {
        $p2 = Get-Proc
        $hwnd = Get-Hwnd $p2
        if ($hwnd -eq 0) { Write-Output "NO_WINDOW"; break }
        $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
        $cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Button)
        $all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, $cond)
        for ($i=0; $i -lt $all.Count; $i++) {
            $e = $all.Item($i)
            Write-Output "BTN[$i] name='$($e.Current.Name)' id='$($e.Current.AutomationId)' enabled=$($e.Current.IsEnabled) off=$($e.Current.IsOffscreen)"
        }
    }
    'invoke' {
        $p2 = Get-Proc
        Invoke-ByName (Get-Hwnd $p2) $Name $Out
    }
    'marker' {
        # check if Marker text exists in current window tree
        $p2 = Get-Proc
        $hwnd = Get-Hwnd $p2
        if ($hwnd -eq 0) { Write-Output "NO_WINDOW"; break }
        $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
        $all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
        $hits = @()
        for ($i=0; $i -lt $all.Count; $i++) {
            $e = $all.Item($i)
            if ($e.Current.Name -and $e.Current.Name.Contains($Marker)) { $hits += "$($e.Current.ControlType.ProgrammaticName):'$($e.Current.Name)':en=$($e.Current.IsEnabled):off=$($e.Current.IsOffscreen)" }
        }
        if ($hits.Count -eq 0) { Write-Output "MARKER_NOT_FOUND '$Marker'" }
        else { Write-Output "MARKER_FOUND '$Marker' -> $($hits -join ' | ')" }
    }
    'resize' {
        $p2 = Get-Proc
        $hwnd = Get-Hwnd $p2
        if ($hwnd -eq 0) { Write-Output "NO_WINDOW"; break }
        [void][Win32Qa]::MoveWindow($hwnd, 10, 10, $W, $H, $true)
        Start-Sleep -Milliseconds 1500
        $p2.Refresh()
        $rect = New-Object Win32Qa+RECT
        [void][Win32Qa]::GetWindowRect($hwnd, [ref]$rect)
        Write-Output "RESIZED request=${W}x${H} actual=$($rect.Right-$rect.Left)x$($rect.Bottom-$rect.Top) alive=$(-not $p2.HasExited) responding=$($p2.Responding) memMB=$([math]::Round($p2.WorkingSet64/1MB))"
    }
    'windows' {
        $p2 = Get-Proc
        $rootEl = [System.Windows.Automation.AutomationElement]::RootElement
        $cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $p2.Id)
        $wins = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
        for ($i = 0; $i -lt $wins.Count; $i++) {
            $w = $wins.Item($i)
            Write-Output "WIN[$i] title='$($w.Current.Name)' type=$($w.Current.ControlType.ProgrammaticName)"
        }
    }
    'click' {
        $p2 = Get-Proc
        $hwnd = Get-Hwnd $p2
        if ($hwnd -eq 0) { Write-Output "NO_WINDOW"; break }
        Add-Type -AssemblyName System.Windows.Forms
        $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
        $all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
        $found = $null
        foreach ($e in $all) { if ($e.Current.Name -eq $Name) { $found = $e; break } }
        if (-not $found) { foreach ($e in $all) { if ($e.Current.Name -and $e.Current.Name.Contains($Name)) { $found = $e; break } } }
        if (-not $found) { Write-Output "NOT_FOUND '$Name'"; break }
        $r = $found.Current.BoundingRectangle
        if ($r.Width -le 0 -or $r.Height -le 0) { Write-Output "NO_RECT name='$($found.Current.Name)' off=$($found.Current.IsOffscreen)"; break }
        [void][Win32Qa]::SetForegroundWindow($hwnd)
        Start-Sleep -Milliseconds 400
        $x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
        [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
        Start-Sleep -Milliseconds 150
        [Win32Qa]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 60
        [Win32Qa]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
        Write-Output "CLICKED name='$($found.Current.Name)' id='$($found.Current.AutomationId)' at=$x,$y enabled=$($found.Current.IsEnabled) off=$($found.Current.IsOffscreen)"
        Start-Sleep -Milliseconds 2000
        if ($Out) {
            $p3 = Get-Proc
            if ($p3) { Write-Snapshot $Out (Get-Hwnd $p3) }
        }
    }
    'invokeparent' {
        $p2 = Get-Proc
        $hwnd = Get-Hwnd $p2
        if ($hwnd -eq 0) { Write-Output "NO_WINDOW"; break }
        $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
        $all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
        $found = $null
        foreach ($e in $all) { if ($e.Current.AutomationId -eq $Name) { $found = $e; break } }
        if (-not $found) { Write-Output "NOT_FOUND id='$Name'"; break }
        $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
        $node = $found
        $clicked = $false
        while ($node -ne $null) {
            try {
                $p = $node.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
                $p.Invoke()
                Write-Output "INVOKED_PARENT id='$($node.Current.AutomationId)' type=$($node.Current.ControlType.ProgrammaticName) name='$($node.Current.Name)'"
                $clicked = $true
                break
            } catch {}
            $node = $walker.GetParent($node)
        }
        if (-not $clicked) { Write-Output "NO_INVOKABLE_PARENT for id='$Name'" }
        Start-Sleep -Milliseconds 1800
        if ($Out) {
            $p3 = Get-Proc
            if ($p3) { Write-Snapshot $Out (Get-Hwnd $p3) }
        }
    }
    'clickidx' {
        $p2 = Get-Proc
        $hwnd = Get-Hwnd $p2
        if ($hwnd -eq 0) { Write-Output "NO_WINDOW"; break }
        Add-Type -AssemblyName System.Windows.Forms
        $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
        $all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
        $idx = [int]$Name
        if ($idx -ge $all.Count) { Write-Output "BAD_IDX $idx count=$($all.Count)"; break }
        $found = $all.Item($idx)
        $r = $found.Current.BoundingRectangle
        if ($r.Width -le 0 -or $r.Height -le 0) { Write-Output "NO_RECT idx=$idx off=$($found.Current.IsOffscreen)"; break }
        [void][Win32Qa]::SetForegroundWindow($hwnd)
        Start-Sleep -Milliseconds 300
        $x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
        [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
        Start-Sleep -Milliseconds 150
        [Win32Qa]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 60
        [Win32Qa]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
        Write-Output "CLICKED idx=$idx name='$($found.Current.Name)' id='$($found.Current.AutomationId)' at=$x,$y"
        Start-Sleep -Milliseconds 2000
        if ($Out) {
            $p3 = Get-Proc
            if ($p3) { Write-Snapshot $Out (Get-Hwnd $p3) }
        }
    }
    'type' {
        $p2 = Get-Proc
        $hwnd = Get-Hwnd $p2
        if ($hwnd -eq 0) { Write-Output "NO_WINDOW"; break }
        $rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($hwnd)
        $all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
        $found = $null
        foreach ($e in $all) { if ($e.Current.AutomationId -eq $Name) { $found = $e; break } }
        if (-not $found) { Write-Output "NOT_FOUND id='$Name'"; break }
        $vp = $null
        try { $vp = $found.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern) } catch {}
        if ($vp) {
            $vp.SetValue($Marker)
            Write-Output "TYPED id='$Name' value='$Marker'"
        } else {
            # fallback: click and send keys
            $r = $found.Current.BoundingRectangle
            $x = [int]($r.X + $r.Width/2); $y = [int]($r.Y + $r.Height/2)
            [void][Win32Qa]::SetForegroundWindow($hwnd)
            Start-Sleep -Milliseconds 300
            [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($x, $y)
            Start-Sleep -Milliseconds 150
            [Win32Qa]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
            Start-Sleep -Milliseconds 60
            [Win32Qa]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
            Start-Sleep -Milliseconds 300
            [System.Windows.Forms.SendKeys]::SendWait($Marker)
            Write-Output "TYPED_SENDKEYS id='$Name'"
        }
        Start-Sleep -Milliseconds 1200
        if ($Out) {
            $p3 = Get-Proc
            if ($p3) { Write-Snapshot $Out (Get-Hwnd $p3) }
        }
    }
    'wmclose' {
        $p2 = Get-Proc
        $hwnd = Get-Hwnd $p2
        if ($hwnd -eq 0) { Write-Output "NO_WINDOW"; break }
        Write-Output "WM_CLOSE sent to $hwnd visible=$([Win32Qa]::IsWindowVisible($hwnd))"
        [void][Win32Qa]::PostMessage($hwnd, 0x0010, [IntPtr]::Zero, [IntPtr]::Zero)
        Start-Sleep -Milliseconds 2500
        $p2.Refresh()
        Write-Output "after-close: alive=$(-not $p2.HasExited) title='$($p2.MainWindowTitle)'"
    }
    'winfo' {
        $p2 = Get-Proc
        $hwnd = Get-Hwnd $p2
        $rect = New-Object Win32Qa+RECT
        [void][Win32Qa]::GetWindowRect($hwnd, [ref]$rect)
        Write-Output "hwnd=$hwnd visible=$([Win32Qa]::IsWindowVisible($hwnd)) rect=$($rect.Left),$($rect.Top),$($rect.Right),$($rect.Bottom) fg=$(([Win32Qa]::GetForegroundWindow()) -eq $hwnd)"
    }
    'close' {
        $p2 = Get-Proc
        if ($p2) { Stop-Process -Id $p2.Id -Force; Write-Output "CLOSED $($p2.Id)" }
        else { Write-Output "NOT_RUNNING" }
    }
    default { Write-Output "UNKNOWN_CMD $Cmd" }
}
