$ErrorActionPreference = 'Continue'
$cs = 'Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True'
$conn = New-Object System.Data.SqlClient.SqlConnection($cs)
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT TOP 5 pc.Value, pc.ContactTypeTitle FROM PersonContacts pc WHERE pc.PersonId = 1002"
$r = $cmd.ExecuteReader()
while ($r.Read()) {
    $v = [string]$r.GetValue(0)
    $hex = [BitConverter]::ToString([System.Text.Encoding]::UTF8.GetBytes($v)) -replace '-', ''
    Write-Output ("Type=[" + $r.GetValue(1) + "] Value=[" + $v + "] Hex=[" + $hex + "]")
}
$r.Close()
$conn.Close()
