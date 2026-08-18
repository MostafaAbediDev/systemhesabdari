param()
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
$proc.Refresh()
$rootEl = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $rootEl.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$sb = New-Object System.Text.StringBuilder
foreach ($e in $all) {
    if ($e.Current.AutomationId -eq 'PersonNameText' -or $e.Current.AutomationId -eq 'ActiveStatusText') {
        [void]$sb.AppendLine("id=$($e.Current.AutomationId) name='$($e.Current.Name)' rect=$($e.Current.BoundingRectangle)")
    }
}
# probe hit-test at right side of name
$name = $null
foreach ($e in $all) { if ($e.Current.AutomationId -eq 'PersonNameText') { $name = $e; break } }
if ($name) {
    $r = $name.Current.BoundingRectangle
    $cy = [int]($r.Y + $r.Height/2)
    for ($dx = 20; $dx -le 120; $dx += 20) {
        $x = [int]($r.Right + $dx)
        $pt = New-Object System.Windows.Point($x, $cy)
        $el = [System.Windows.Automation.AutomationElement]::FromPoint($pt)
        if ($el) { [void]$sb.AppendLine("hit x=$x,y=$cy => type=$($el.Current.ControlType.ProgrammaticName) name='$($el.Current.Name)' id='$($el.Current.AutomationId)'") }
        else { [void]$sb.AppendLine("hit x=$x,y=$cy => NONE") }
    }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_editpos.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
