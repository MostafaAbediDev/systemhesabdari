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

Write-Output "=== Branches LIKE شعبه ۵۰۰% ==="
RunQ "SELECT Id, Title, NationalId FROM Branches WHERE Title LIKE N'%شعبه ۵۰۰%' OR Title LIKE N'%QA%'"

Write-Output "=== Codes recent (last 5) ==="
RunQ "SELECT TOP 5 * FROM Codes ORDER BY Id DESC"

Write-Output "=== Codes containing QA ==="
RunQ "SELECT * FROM Codes WHERE Code LIKE N'%QA%' OR ManualCode LIKE N'%QA%'"

$conn.Close()
