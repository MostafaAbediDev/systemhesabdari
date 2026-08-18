param([int]$Cycles = 10)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

function Get-Tree {
    $p = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $p) { return @() }
    $h = $p.MainWindowHandle
    if ($h -eq 0) { return @() }
    $root = [System.Windows.Automation.AutomationElement]::FromHandle($h)
    return $root.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
}

function Invoke-ByName([string]$name) {
    $all = Get-Tree
    for ($i = 0; $i -lt $all.Count; $i++) {
        $e = $all.Item($i)
        if ($e.Current.Name -eq $name) {
            try {
                $node = $e
                $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
                while ($node -ne $null) {
                    try {
                        $p = $node.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
                        $p.Invoke()
                        return "OK:" + $name
                    } catch {}
                    $node = $walker.GetParent($node)
                }
                return "NO_INVOKABLE:" + $name
            } catch { return "ERR:" + $name }
        }
    }
    return "NOT_FOUND:" + $name
}

$p0 = Get-Process -Name Taadol | Select-Object -First 1
$mem0 = [math]::Round($p0.WorkingSet64 / 1MB)
$start = Get-Date
for ($c = 1; $c -le $Cycles; $c++) {
    $r1 = Invoke-ByName "لیست اشخاص"
    Start-Sleep -Milliseconds 800
    $r2 = Invoke-ByName "شخص جدید"
    Start-Sleep -Milliseconds 800
    if ($c % 3 -eq 0) {
        $p = Get-Process -Name Taadol | Select-Object -First 1
        $mem = [math]::Round($p.WorkingSet64 / 1MB)
        Write-Output ("cycle=" + $c + " mem=" + $mem + " responding=" + $p.Responding + " r1=" + $r1 + " r2=" + $r2)
    }
}
$elapsed = ((Get-Date) - $start).TotalSeconds
$p1 = Get-Process -Name Taadol | Select-Object -First 1
$mem1 = [math]::Round($p1.WorkingSet64 / 1MB)
$alive = if ($p1.HasExited) { 'False' } else { 'True' }
$msg = "CYCLES=" + $Cycles + " timeSec=" + [math]::Round($elapsed, 1) + " mem0=" + $mem0 + " mem1=" + $mem1 + " deltaMB=" + ($mem1 - $mem0) + " responding=" + $p1.Responding + " alive=" + $alive
Write-Output $msg
