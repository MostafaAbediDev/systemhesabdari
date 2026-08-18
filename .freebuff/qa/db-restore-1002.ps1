$ErrorActionPreference = 'Continue'
$cs = 'Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True'
$conn = New-Object System.Data.SqlClient.SqlConnection($cs)
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandText = "UPDATE Persons SET CreditLimit = 10001000, AvailableCredit = 10001000 WHERE Id = 1002"
$n = $cmd.ExecuteNonQuery()
Write-Output ("RESTORED_ROWS=" + $n)

$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = "SELECT Id, CreditLimit, AvailableCredit FROM Persons WHERE Id = 1002"
$r = $cmd2.ExecuteReader()
while ($r.Read()) {
    Write-Output ("Id=" + $r.GetValue(0) + " CreditLimit=" + $r.GetValue(1) + " AvailableCredit=" + $r.GetValue(2))
}
$r.Close()
$conn.Close()
