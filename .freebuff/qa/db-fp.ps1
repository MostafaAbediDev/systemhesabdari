$ErrorActionPreference = 'Stop'
$cs = 'Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True'
$conn = New-Object System.Data.SqlClient.SqlConnection($cs)
$conn.Open()

$cmd0 = $conn.CreateCommand()
$cmd0.CommandText = "SELECT COUNT(*) FROM FinancialPeriods"
Write-Output ("FinancialPeriods total: " + $cmd0.ExecuteScalar())

$cmd1 = $conn.CreateCommand()
$cmd1.CommandText = "SELECT Id, Title, StartDate, EndDate FROM FinancialPeriods WHERE Title LIKE N'%اصلی%' OR Title LIKE N'%QA%'"
$r = $cmd1.ExecuteReader()
while ($r.Read()) { Write-Output ("FP Id=[" + $r['Id'] + "] Title=[" + $r['Title'] + "] Start=[" + $r['StartDate'] + "] End=[" + $r['EndDate'] + "]") }
$r.Close()

$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = "UPDATE FinancialPeriods SET Title = N'اصلی' WHERE Title = N'اصلی QA'"
$n = $cmd2.ExecuteNonQuery()
Write-Output ("Restored: " + $n)
$conn.Close()
