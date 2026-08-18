$ErrorActionPreference = 'Stop'
$cs = 'Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True'
$conn = New-Object System.Data.SqlClient.SqlConnection($cs)
$conn.Open()

# 1) restore branch title
$cmd = $conn.CreateCommand()
$cmd.CommandText = "UPDATE Branches SET Title = N'شعبه ۵۰۰' WHERE Id = 1500"
$n = $cmd.ExecuteNonQuery()
Write-Output ("Branches updated: " + $n)

# 2) delete the QA-EDIT-500 code row (created during QA test)
$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = "DELETE FROM Codes WHERE Id = 11023 AND Value LIKE N'%QA%'"
$n2 = $cmd2.ExecuteNonQuery()
Write-Output ("Codes deleted: " + $n2)

# 3) verify
$cmd3 = $conn.CreateCommand()
$cmd3.CommandText = "SELECT Id, Title FROM Branches WHERE Id = 1500"
$r = $cmd3.ExecuteReader()
while ($r.Read()) { Write-Output ("VERIFY branch: Id=[" + $r['Id'] + "] Title=[" + $r['Title'] + "]") }
$r.Close()

$cmd4 = $conn.CreateCommand()
$cmd4.CommandText = "SELECT COUNT(*) FROM Codes WHERE OwnerId = 1500"
$c = $cmd4.ExecuteScalar()
Write-Output ("VERIFY codes for owner 1500: " + $c)

$conn.Close()
Write-Output "CLEANUP DONE"
