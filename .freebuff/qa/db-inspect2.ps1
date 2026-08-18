$ErrorActionPreference = 'Continue'
$cs = 'Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True'
$conn = New-Object System.Data.SqlClient.SqlConnection($cs)
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT Id, Value, OwnerId, OwnerType, IsDeleted, CreationDate FROM Codes WHERE OwnerId = 1500"
$r = $cmd.ExecuteReader()
while ($r.Read()) {
    Write-Output ("Code Id=[" + $r['Id'] + "] Value=[" + $r['Value'] + "] IsDeleted=[" + $r['IsDeleted'] + "] Created=[" + $r['CreationDate'] + "]")
}
$r.Close()

$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = "SELECT Id, Title, NationalId, CreatedDate FROM Branches WHERE Id = 1500"
$r2 = $cmd2.ExecuteReader()
while ($r2.Read()) {
    Write-Output ("Branch Id=[" + $r2['Id'] + "] Title=[" + $r2['Title'] + "] NationalId=[" + $r2['NationalId'] + "]")
}
$r2.Close()
$conn.Close()
