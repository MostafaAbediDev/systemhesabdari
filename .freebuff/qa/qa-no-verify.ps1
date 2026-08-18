$ErrorActionPreference = 'Continue'
$taadolPid = $env:TAADOL_PID
if (-not $taadolPid) { $taadolPid = '19540' }
$env:TAADOL_PID = $taadolPid
. 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\.freebuff\qa\uia-lib.ps1'

$win = Get-TaadolWindow
if (-not $win) { Write-Output "WINDOW NOT FOUND"; exit 1 }
Set-Foreground $win
Start-Sleep -Milliseconds 300

# 1) click انصراف to reopen dirty dialog
$cancel = Find-ByName $win 'انصراف'
if (-not $cancel) { Write-Output "CANCEL NOT FOUND"; exit 1 }
Click-El $cancel
Wait-Ms 1000

$dlg = $win.FindFirst([System.Windows.Automation.TreeScope]::Children, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window)))
if (-not $dlg) { Write-Output "DIALOG NOT FOUND"; exit 1 }
Write-Output ("DIALOG=[" + $dlg.Current.Name + "]")

# 2) click No
$no = $dlg.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'No')))
if (-not $no) { Write-Output "NO BTN NOT FOUND"; exit 1 }
Click-El $no
Wait-Ms 1500

# 3) modal closed?
$modalStill = Find-ByAId $win 'SaveButton'
Write-Output ("MODAL_OPEN_AFTER_NO=" + [bool]$modalStill)

# 4) DB title check via script (avoids bash $ mangling)
$cs = 'Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True'
$conn = New-Object System.Data.SqlClient.SqlConnection($cs)
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT Title FROM Branches WHERE Id=1500"
$r = $cmd.ExecuteReader()
while ($r.Read()) { Write-Output ("DB_TITLE=[" + $r.GetValue(0) + "]") }
$r.Close()
$conn.Close()
Write-Output "DONE"
