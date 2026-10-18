
param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    [switch]$Minimized,
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$AppArgs
)

$ErrorActionPreference = "Stop"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet non trovato: installa .NET 8 SDK da https://dotnet.microsoft.com/download"
}
if (-not (dotnet --list-sdks 2>$null)) {
    throw "Nessun .NET SDK trovato (solo i runtime). Installa il .NET 8 SDK: winget install Microsoft.DotNet.SDK.8"
}

$project = Join-Path $PSScriptRoot "DeepLocal.csproj"

$appArgs = @()
if ($Minimized) { $appArgs += "--minimized" }
if ($AppArgs) { $appArgs += $AppArgs }

$dotnetArgs = @("run", "--project", $project, "-c", $Configuration)
if ($appArgs.Count -gt 0) { $dotnetArgs += "--"; $dotnetArgs += $appArgs }

& dotnet @dotnetArgs
exit $LASTEXITCODE
