$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$proc = Get-Process -Name Taadol -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $proc) { Write-Output "NOT_RUNNING"; exit 1 }
$sb = New-Object System.Text.StringBuilder
$rootEl = [System.Windows.Automation.AutomationElement]::RootElement
for ($x = 1315; $x -le 1400; $x += 8) {
    try {
        $el = [System.Windows.Automation.AutomationElement]::FromPoint([System.Windows.Point]::new($x, 263))
        if ($el) {
            $r = $el.Current.BoundingRectangle
            [void]$sb.AppendLine("x=$x -> type=$($el.Current.ControlType.ProgrammaticName) name='$($el.Current.Name)' id='$($el.Current.AutomationId)' rect=$r")
        }
    } catch { [void]$sb.AppendLine("x=$x ERR") }
}
[IO.File]::WriteAllText('E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\fx_hitband.txt', $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE"
