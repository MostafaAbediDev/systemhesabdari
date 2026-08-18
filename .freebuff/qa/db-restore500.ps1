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

# restore title
$cmd = $conn.CreateCommand()
$cmd.CommandText = "UPDATE Branches SET Title = N'شعبه ۵۰۰' WHERE Id = 1500"
$n = $cmd.ExecuteNonQuery()
Write-Output ("TITLE_RESTORED_ROWS=" + $n)

# delete QA test code row (OwnerType=1, Value like QA-EDIT2)
$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = "DELETE FROM Codes WHERE OwnerId = 1500 AND OwnerType = 1"
$n2 = $cmd2.ExecuteNonQuery()
Write-Output ("CODE_DELETED_ROWS=" + $n2)

Write-Output "=== VERIFY ==="
RunQ "SELECT Id, Title, NationalId, IsActive FROM Branches WHERE Id = 1500"
RunQ "SELECT Id, Value, OwnerId, OwnerType FROM Codes WHERE OwnerId = 1500 ORDER BY Id"
RunQ "SELECT COUNT(*) AS BranchCount FROM Branches"

$conn.Close()
