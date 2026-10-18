
param(
    [ValidateSet("en", "it")]
    [string]$Lang = "en",
    [switch]$NoLaunch
)

$ErrorActionPreference = "Stop"

if (-not (dotnet --list-sdks 2>$null)) {
    throw "Nessun .NET SDK trovato (solo i runtime). Installa il .NET 8 SDK: winget install Microsoft.DotNet.SDK.8"
}

$build = Join-Path $PSScriptRoot "installer\build.ps1"
& $build

$suffix = if ($Lang -eq "it") { "ITA" } else { "EN" }
$setup = Join-Path $PSScriptRoot ("installer\Output\DeepLocal_Setup_{0}.exe" -f $suffix)
if (-not (Test-Path $setup)) { throw "Installer non trovato: $setup" }

if ($NoLaunch) {
    Write-Host "Installer pronto: $setup"
} else {
    Write-Host "Avvio l'installer: $setup"
    Start-Process -FilePath $setup
}
