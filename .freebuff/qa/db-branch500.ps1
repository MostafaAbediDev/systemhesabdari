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

Write-Output "=== Branches Id=1500 (شعبه ۵۰۰) ==="
RunQ "SELECT * FROM Branches WHERE Id = 1500"

Write-Output "=== Codes for OwnerId=1500 ==="
RunQ "SELECT * FROM Codes WHERE OwnerId = 1500"

Write-Output "=== Branch Count ==="
RunQ "SELECT COUNT(*) AS BranchCount FROM Branches"

$conn.Close()
