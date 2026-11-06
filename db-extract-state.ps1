param(
    [string]$TargetEnv = "Staging", # "Staging", "Local", "Production", or custom conn string
    [string]$OutputPath = ""
)

$ErrorActionPreference = "Stop"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host " Extracting Database Schema State (DACPAC)" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

# Locate SqlPackage.exe
$SqlPackagePaths = @(
    "C:\Program Files\Microsoft Visual Studio\18\Professional\Common7\IDE\Extensions\Microsoft\SQLDB\DAC\SqlPackage.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\Extensions\Microsoft\SQLDB\DAC\SqlPackage.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\Extensions\Microsoft\SQLDB\DAC\SqlPackage.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\Extensions\Microsoft\SQLDB\DAC\SqlPackage.exe",
    "C:\Program Files\Microsoft SQL Server\160\DAC\bin\SqlPackage.exe",
    "C:\Program Files\Microsoft SQL Server\150\DAC\bin\SqlPackage.exe"
)

$SqlPackage = $null
foreach ($path in $SqlPackagePaths) {
    if (Test-Path $path) {
        $SqlPackage = $path
        break
    }
}

if (-not $SqlPackage) {
    $found = Get-Command "sqlpackage.exe" -ErrorAction SilentlyContinue
    if ($found) { $SqlPackage = $found.Source }
}

if (-not $SqlPackage) {
    Write-Host "ERROR: SqlPackage.exe not found! Please ensure SSDT or DacFx is installed." -ForegroundColor Red
    exit 1
}

Write-Host "Using SqlPackage: $SqlPackage" -ForegroundColor Gray

# Determine Output DACPAC path
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $PSScriptRoot "LIMS.Database\LIMS.Database.dacpac"
}

$OutputDir = Split-Path $OutputPath
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

# Resolve connection string
$SecretsFile = Join-Path $PSScriptRoot "LIMSApi\appsettings.Secrets.json"
$connStr = ""
if (Test-Path $SecretsFile) {
    $secrets = Get-Content $SecretsFile -Raw | ConvertFrom-Json
    if ($TargetEnv -eq "Staging" -and $secrets.ConnectionStrings.LimsDevConnection) {
        $connStr = $secrets.ConnectionStrings.LimsDevConnection
    } elseif ($TargetEnv -eq "Production" -and $secrets.ConnectionStrings.ProductionConnection) {
        $connStr = $secrets.ConnectionStrings.ProductionConnection
    } elseif ($TargetEnv -eq "Local" -and $secrets.ConnectionStrings.DefaultConnection) {
        $connStr = $secrets.ConnectionStrings.DefaultConnection
    } elseif ($secrets.ConnectionStrings.$TargetEnv) {
        $connStr = $secrets.ConnectionStrings.$TargetEnv
    } else {
        $connStr = $TargetEnv
    }
} else {
    $connStr = $TargetEnv
}

if ([string]::IsNullOrWhiteSpace($connStr)) {
    Write-Host "ERROR: Could not resolve connection string for target: $TargetEnv" -ForegroundColor Red
    exit 1
}

Write-Host "Extracting declarative schema from target database to:" -ForegroundColor Green
Write-Host "  -> $OutputPath" -ForegroundColor Yellow

& $SqlPackage /Action:Extract `
    /SourceConnectionString:$connStr `
    /TargetFile:$OutputPath `
    /p:IgnoreExtendedProperties=true `
    /p:ExtractApplicationScopedExecutablesOnly=false

if ($LASTEXITCODE -eq 0) {
    Write-Host "SUCCESS: Database schema state extracted into DACPAC!" -ForegroundColor Green
} else {
    Write-Host "ERROR: SqlPackage extraction failed with exit code $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}

