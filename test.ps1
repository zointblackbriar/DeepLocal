# Esegue i test unitari del progetto.
# Uso: powershell -ExecutionPolicy Bypass -File test.ps1

$ErrorActionPreference = "Stop"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet non trovato: installa .NET 8 SDK da https://dotnet.microsoft.com/download"
}
if (-not (dotnet --list-sdks 2>$null)) {
    throw "Nessun .NET SDK trovato (solo i runtime). Installa il .NET 8 SDK: winget install Microsoft.DotNet.SDK.8"
}

& dotnet test (Join-Path $PSScriptRoot "tests\DeepLocal.Tests\DeepLocal.Tests.csproj")
exit $LASTEXITCODE
