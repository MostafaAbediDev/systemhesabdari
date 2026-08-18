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

Write-Output "=== ALL persons: count + sums ==="
RunQ "SELECT COUNT(*) AS Cnt, SUM(CreditLimit) AS SumDebit, SUM(AvailableCredit) AS SumCredit FROM Persons WHERE IsDeleted = 0"

Write-Output "=== person 1002 current ==="
RunQ "SELECT Id, CreditLimit, AvailableCredit FROM Persons WHERE Id = 1002"

$conn.Close()
