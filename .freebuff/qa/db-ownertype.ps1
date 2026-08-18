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

Write-Output "=== Codes by OwnerType ==="
RunQ "SELECT OwnerType, COUNT(*) AS Cnt FROM Codes GROUP BY OwnerType ORDER BY OwnerType"

Write-Output "=== Codes OwnerType=1 (Branch) first 5 ==="
RunQ "SELECT TOP 5 Id, Value, OwnerId, OwnerType FROM Codes WHERE OwnerType = 1"

Write-Output "=== Codes OwnerType=2 (Person) with OwnerId=1500 ==="
RunQ "SELECT Id, Value, OwnerId, OwnerType FROM Codes WHERE OwnerId = 1500"

$conn.Close()
