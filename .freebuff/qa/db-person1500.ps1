$ErrorActionPreference = 'Continue'
$cs = 'Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True'
$conn = New-Object System.Data.SqlClient.SqlConnection($cs)
$conn.Open()

function RunQ($sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    $r = $cmd.ExecuteReader()
    while ($r.Read()) {
        $line = ''
        for ($i = 0; $i -lt $r.FieldCount; $i++) {
            $line += $r.GetName($i) + '=[' + $r.GetValue($i) + '] '
        }
        Write-Output $line
    }
    $r.Close()
}

Write-Output "=== Persons with Id in (1500) ==="
RunQ "SELECT Id, FirstName, LastName FROM Persons WHERE Id = 1500"

Write-Output "=== Branches with Id=1500 ==="
RunQ "SELECT Id, Title FROM Branches WHERE Id = 1500"

Write-Output "=== Codes OwnerType=1 for OwnerId=1500 ==="
RunQ "SELECT Id, Value, OwnerId, OwnerType FROM Codes WHERE OwnerId = 1500 AND OwnerType = 1"

Write-Output "=== Codes OwnerType=2 count with OwnerId > 1000 (branch-like ids) ==="
RunQ "SELECT COUNT(*) AS Cnt FROM Codes c LEFT JOIN Branches b ON b.Id = c.OwnerId WHERE c.OwnerType = 2 AND b.Id IS NOT NULL"

$conn.Close()
