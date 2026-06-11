param(
    [switch]$StartApi
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

Write-Host "ATENCAO: este script apaga somente o banco local de desenvolvimento configurado no projeto."
Write-Host "Nao use este script em Production."

$apiProcesses = Get-CimInstance Win32_Process |
    Where-Object {
        $_.Name -match "dotnet|ZeroFrame.API" -and
        $_.CommandLine -match "ZeroFrame.API"
    }

foreach ($process in $apiProcesses) {
    Write-Host "Parando execucao antiga da API. PID: $($process.ProcessId)"
    Stop-Process -Id $process.ProcessId -Force -ErrorAction SilentlyContinue
}

Push-Location $root
try {
    dotnet restore
    dotnet build

    dotnet ef database drop --force --project ZeroFrame.Infra.Data --startup-project ZeroFrame.API
    dotnet ef database update --project ZeroFrame.Infra.Data --startup-project ZeroFrame.API

    Write-Host "Banco local de desenvolvimento recriado com migrations."
    Write-Host "Para executar a seed, inicie a API em Development:"
    Write-Host "dotnet run --project ZeroFrame.API --launch-profile http"

    if ($StartApi) {
        dotnet run --project ZeroFrame.API --launch-profile http
    }
}
finally {
    Pop-Location
}
