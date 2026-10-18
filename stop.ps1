$ErrorActionPreference = "Stop"

$procs = Get-Process -Name "DeepLocal" -ErrorAction SilentlyContinue
if (-not $procs) {
    Write-Host "DeepLocal non e' in esecuzione."
    exit 0
}

$ids = ($procs | Select-Object -ExpandProperty Id) -join ", "
$procs | Stop-Process -Force
Write-Host "DeepLocal terminato (PID: $ids)."
