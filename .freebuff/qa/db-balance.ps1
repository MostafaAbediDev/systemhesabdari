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

Write-Output "=== Persons with CreditLimit or AvailableCredit > 0 (top 10) ==="
RunQ "SELECT TOP 10 Id, FirstName, LastName, CreditLimit, AvailableCredit FROM Persons WHERE (CreditLimit > 0 OR AvailableCredit > 0) ORDER BY Id"

Write-Output "=== Person count with balance ==="
RunQ "SELECT COUNT(*) AS Cnt FROM Persons WHERE (CreditLimit > 0 OR AvailableCredit > 0)"

$conn.Close()
