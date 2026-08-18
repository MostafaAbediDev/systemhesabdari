$ErrorActionPreference = 'Continue'
$cs = 'Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True'
$conn = New-Object System.Data.SqlClient.SqlConnection($cs)
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT Id, NationalCode FROM Persons WHERE Id = 1002"
$r = $cmd.ExecuteReader()
while ($r.Read()) {
    $nc = $r.GetValue(1)
    $hex = [BitConverter]::ToString([System.Text.Encoding]::UTF8.GetBytes([string]$nc)) -replace '-', ''
    Write-Output ("Id=" + $r.GetValue(0) + " NationalCode=[" + $nc + "] Hex=[" + $hex + "]")
}
$r.Close()
$conn.Close()
