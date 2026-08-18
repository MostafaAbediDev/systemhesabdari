$ErrorActionPreference = 'Continue'
$exe = 'E:\NewVersiongithub\githubeVersionMerge\Code\systemhesabdari\src\FrontEndWPF\Taadol\Taadol\bin\Debug\net9.0-windows\Taadol.exe'
$existing = Get-Process -Name Taadol -ErrorAction SilentlyContinue
if ($existing) {
    foreach ($e in $existing) {
        Write-Output ("EXISTING PID=" + $e.Id + " TITLE=[" + $e.MainWindowTitle + "] RESP=" + $e.Responding)
    }
} else {
    $p = Start-Process -FilePath $exe -PassThru
    Start-Sleep -Seconds 10
    $p2 = Get-Process -Id $p.Id -ErrorAction SilentlyContinue
    if ($p2) {
        Write-Output ("ALIVE PID=" + $p2.Id + " TITLE=[" + $p2.MainWindowTitle + "] RESP=" + $p2.Responding)
    } else {
        Write-Output "DIED"
    }
}
# startup log tail
$log = Join-Path $env:TEMP 'taadol-startup.log'
if (Test-Path $log) {
    Write-Output "--- STARTUP LOG (tail 25) ---"
    Get-Content $log -Tail 25
} else {
    Write-Output "NO STARTUP LOG"
}
