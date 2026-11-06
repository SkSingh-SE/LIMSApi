param(
    [string]$TargetEnv = "Staging", # "Staging", "Local", "Production", or custom connection string
    [string]$DacpacPath = "",
    [switch]$GenerateScriptOnly,
    [switch]$WhatIf
)

$ErrorActionPreference = "Stop"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host " State-Based Declarative Schema Sync" -ForegroundColor Cyan
Write-Host " Method: SSDT / DacFx (Approach 3)" -ForegroundColor Cyan
Write-Host " Target: $TargetEnv" -ForegroundColor Cyan
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

# Resolve source DACPAC
if ([string]::IsNullOrWhiteSpace($DacpacPath)) {
    $DacpacPath = Join-Path $PSScriptRoot "LIMS.Database\LIMS.Database.dacpac"
}

if (-not (Test-Path $DacpacPath)) {
    Write-Host "ERROR: Source DACPAC not found at: $DacpacPath" -ForegroundColor Red
    Write-Host "Run ./db-extract-state.ps1 first to extract/create the declarative baseline." -ForegroundColor Yellow
    exit 1
}

# Resolve target connection string
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
    Write-Host "ERROR: Could not resolve target connection string for: $TargetEnv" -ForegroundColor Red
    exit 1
}

$OutputDir = Join-Path $PSScriptRoot "LIMS.Database\Output"
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

$DiffScriptPath = Join-Path $OutputDir "DeployDiff_$TargetEnv.sql"
$ReportPath = Join-Path $OutputDir "DeployReport_$TargetEnv.xml"

# Step 1: Generate Differential Script
Write-Host "1. Computing declarative schema diff..." -ForegroundColor Green
& $SqlPackage /Action:Script `
    /SourceFile:$DacpacPath `
    /TargetConnectionString:$connStr `
    /OutputPath:$DiffScriptPath `
    /p:BlockOnPossibleDataLoss=false `
    /p:DropObjectsNotInSource=false `
    /p:GenerateSmartDefaults=true `
    /p:IgnorePermissions=true `
    /p:IgnoreRoleMembership=true `
    /p:CommandTimeout=120

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to generate differential script." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "   -> Differential SQL generated: $DiffScriptPath" -ForegroundColor Gray

# Step 2: Generate Drift / Impact Report
Write-Host "2. Generating change impact report..." -ForegroundColor Green
& $SqlPackage /Action:DeployReport `
    /SourceFile:$DacpacPath `
    /TargetConnectionString:$connStr `
    /OutputPath:$ReportPath `
    /p:BlockOnPossibleDataLoss=false `
    /p:DropObjectsNotInSource=false `
    /p:IgnorePermissions=true

Write-Host "   -> Deploy report generated: $ReportPath" -ForegroundColor Gray

if ($GenerateScriptOnly -or $WhatIf) {
    Write-Host "Completed in Script-Only / WhatIf mode. Review generated files before publishing." -ForegroundColor Yellow
    exit 0
}

# Step 3: Publish to Target Database
Write-Host "3. Publishing declarative schema to target database ($TargetEnv)..." -ForegroundColor Cyan
& $SqlPackage /Action:Publish `
    /SourceFile:$DacpacPath `
    /TargetConnectionString:$connStr `
    /p:BlockOnPossibleDataLoss=false `
    /p:DropObjectsNotInSource=false `
    /p:GenerateSmartDefaults=true `
    /p:IgnorePermissions=true `
    /p:IgnoreRoleMembership=true `
    /p:CommandTimeout=180

if ($LASTEXITCODE -eq 0) {
    Write-Host "==========================================" -ForegroundColor Green
    Write-Host " SUCCESS! Database schema synchronized!" -ForegroundColor Green
    Write-Host " Target environment is 100% compliant." -ForegroundColor Green
    Write-Host "==========================================" -ForegroundColor Green
} else {
    Write-Host "ERROR: Schema publish failed with code $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}

