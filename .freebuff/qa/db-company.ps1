$ErrorActionPreference = 'Stop'
$cs = 'Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True'
$conn = New-Object System.Data.SqlClient.SqlConnection($cs)
$conn.Open()

$cmd0 = $conn.CreateCommand()
$cmd0.CommandText = "SELECT COUNT(*) FROM Companies"
Write-Output ("Companies total: " + $cmd0.ExecuteScalar())

$cmd1 = $conn.CreateCommand()
$cmd1.CommandText = "SELECT Id, Title, IsActive FROM Companies WHERE Title LIKE N'%ایران خودرد%'"
$r = $cmd1.ExecuteReader()
while ($r.Read()) { Write-Output ("Company Id=[" + $r['Id'] + "] Title=[" + $r['Title'] + "] Active=[" + $r['IsActive'] + "]") }
$r.Close()

$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = "UPDATE Companies SET Title = N'ایران خودرد' WHERE Title = N'ایران خودرد QA'"
$n = $cmd2.ExecuteNonQuery()
Write-Output ("Restored: " + $n)

$conn.Close()
