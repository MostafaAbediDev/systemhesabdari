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
            $v = $r.GetValue($i)
            if ($v -is [string]) {
                $b = [System.Text.Encoding]::UTF8.GetBytes($v)
                $v = 'U:' + ([BitConverter]::ToString($b) -replace '-', '')
            }
            $line += $r.GetName($i) + '=[' + $v + '] '
        }
        Write-Output $line
    }
    $r.Close()
}

Write-Output "=== Person 1002 (UTF8 hex) ==="
RunQ "SELECT Id, FirstName, LastName, CreditLimit, AvailableCredit FROM Persons WHERE Id = 1002"

Write-Output "=== Code for person 1002 ==="
RunQ "SELECT Id, Value, OwnerId, OwnerType FROM Codes WHERE OwnerId = 1002 AND OwnerType = 2"

Write-Output "=== Set test balance 1234567 ==="
$cmd = $conn.CreateCommand()
$cmd.CommandText = "UPDATE Persons SET CreditLimit = 1234567, AvailableCredit = 0 WHERE Id = 1002"
$n = $cmd.ExecuteNonQuery()
Write-Output ("UPDATED=" + $n)
RunQ "SELECT Id, CreditLimit, AvailableCredit FROM Persons WHERE Id = 1002"

$conn.Close()
