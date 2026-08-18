$ErrorActionPreference = 'Continue'
$cs = 'Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True'
$conn = New-Object System.Data.SqlClient.SqlConnection($cs)
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT Id, Title FROM Branches WHERE Id = 1500"
$r = $cmd.ExecuteReader()
while ($r.Read()) { Write-Output ("Id=[" + $r['Id'] + "] Title=[" + $r['Title'] + "]") }
$r.Close()
$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = "SELECT COUNT(*) FROM Codes WHERE OwnerId = 1500"
Write-Output ("Codes for owner 1500: " + $cmd2.ExecuteScalar())
$conn.Close()
