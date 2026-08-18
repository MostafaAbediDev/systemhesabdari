param(
    [int]$X = 1057,
    [int]$Y = 303,
    [string]$Out = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\verify_hit.txt'
)
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName WindowsBase
$sb = New-Object System.Text.StringBuilder
$pt = New-Object System.Windows.Point($X, $Y)
$el = [System.Windows.Automation.AutomationElement]::FromPoint($pt)
if (-not $el) { [void]$sb.AppendLine("NO_ELEMENT at $X,$Y") }
else {
    [void]$sb.AppendLine("TOP type=$($el.Current.ControlType.ProgrammaticName) name='$($el.Current.Name)' id='$($el.Current.AutomationId)' off=$($el.Current.IsOffscreen)")
    $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
    $node = $el
    $depth = 0
    while ($node -ne $null -and $depth -lt 12) {
        $t = $node.Current.ControlType.ProgrammaticName
        $n = $node.Current.Name
        $id = $node.Current.AutomationId
        [void]$sb.AppendLine("  ANC[$depth] $t name='$n' id='$id' en=$($node.Current.IsEnabled) off=$($node.Current.IsOffscreen)")
        $node = $walker.GetParent($node)
        $depth++
    }
}
[IO.File]::WriteAllText($Out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output "DONE -> $Out"
