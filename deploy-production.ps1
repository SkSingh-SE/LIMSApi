param(
    [string]$Password = "ZAQwsxCDE@91"
)

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host " Building .NET Backend API (Production)..." -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

Set-Location $PSScriptRoot
dotnet publish LIMSApi/LIMSApi.csproj -c Release --no-restore -o LIMSApi/bin/Release/net8.0/publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed! Deployment cancelled." -ForegroundColor Red
    exit 1
}

# Copy web.config with OutOfProcess hosting model
$WebConfigSrc = Join-Path $PSScriptRoot "LIMSApi\web.config"
$WebConfigDst = Join-Path $PSScriptRoot "LIMSApi\bin\Release\net8.0\publish\web.config"
if (Test-Path $WebConfigSrc) {
    Copy-Item $WebConfigSrc $WebConfigDst -Force
}

# Prepare appsettings.Secrets.json with Production (LimsDbConnection) as DefaultConnection
$SecretsSrc = Join-Path $PSScriptRoot "LIMSApi\appsettings.Secrets.json"
$SecretsDst = Join-Path $PSScriptRoot "LIMSApi\bin\Release\net8.0\publish\appsettings.Secrets.json"
if (Test-Path $SecretsSrc) {
    $secrets = Get-Content $SecretsSrc -Raw | ConvertFrom-Json
    if ($secrets.ConnectionStrings.LimsDbConnection) {
        $secrets.ConnectionStrings.DefaultConnection = $secrets.ConnectionStrings.LimsDbConnection
    }
    $secrets | ConvertTo-Json -Depth 10 | Set-Content $SecretsDst -Encoding UTF8
}

Write-Host "==========================================" -ForegroundColor Green
Write-Host " Deploying to IIS Production Server..." -ForegroundColor Green
Write-Host " Site: dmspl91-001-site3" -ForegroundColor Green
Write-Host " URL: http://dmspl91-001-site3.ntempurl.com/" -ForegroundColor Green
Write-Host " Database: db_ac76b4_limsdb" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green

$MSDeployPath = "C:\Program Files (x86)\IIS\Microsoft Web Deploy V3\msdeploy.exe"
if (-not (Test-Path $MSDeployPath)) {
    $MSDeployPath = "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe"
}

$SourcePath = Join-Path $PSScriptRoot "LIMSApi\bin\Release\net8.0\publish"

# Create app_offline.htm to safely release DLL locks
$AppOfflineFile = Join-Path $SourcePath "app_offline.htm"
Set-Content -Path $AppOfflineFile -Value "<html><body><h2>Deploying update, please wait...</h2></body></html>" -Encoding UTF8

$deploySuccess = $false
$maxAttempts = 3

for ($attempt = 1; $attempt -le $maxAttempts; $attempt++) {
    Write-Host "Deployment Attempt $attempt of $maxAttempts..." -ForegroundColor Cyan

    & $MSDeployPath -verb:sync `
        -source:contentPath="$SourcePath" `
        -dest:contentPath="dmspl91-001-site3",wmsvc="https://win6046.site4now.net:8172/MsDeploy.axd?site=dmspl91-001-site3",userName="dmspl91-001",password="$Password",authtype="Basic" `
        -enableRule:AppOffline `
        -skip:objectName=dirPath,absolutePath=".*\\logs" `
        -skip:objectName=filePath,absolutePath=".*\\logs\\.*" `
        -skip:objectName=dirPath,absolutePath=".*\\Uploads" `
        -skip:objectName=filePath,absolutePath=".*\\Uploads\\.*" `
        -skip:objectName=dirPath,absolutePath=".*\\wwwroot\\Uploads" `
        -skip:objectName=filePath,absolutePath=".*\\wwwroot\\Uploads\\.*" `
        -allowUntrusted

    if ($LASTEXITCODE -eq 0) {
        $deploySuccess = $true
        break
    } else {
        Write-Host "Attempt $attempt encountered lock, waiting 5 seconds for IIS process to release..." -ForegroundColor Yellow
        Start-Sleep -Seconds 5
    }
}

# Remove app_offline.htm so the site comes back online immediately
if (Test-Path $AppOfflineFile) {
    Remove-Item $AppOfflineFile -Force
}

# Sync removal of app_offline.htm to bring site online
& $MSDeployPath -verb:delete `
    -dest:contentPath="dmspl91-001-site3/app_offline.htm",wmsvc="https://win6046.site4now.net:8172/MsDeploy.axd?site=dmspl91-001-site3",userName="dmspl91-001",password="$Password",authtype="Basic" `
    -allowUntrusted 2>$null

if ($deploySuccess) {
    Write-Host "==========================================" -ForegroundColor Green
    Write-Host " SUCCESS! Backend API deployed to Production (Site-3)!" -ForegroundColor Green
    Write-Host " Live URL: http://dmspl91-001-site3.ntempurl.com/" -ForegroundColor Green
    Write-Host "==========================================" -ForegroundColor Green
} else {
    Write-Host "Deployment failed after $maxAttempts attempts." -ForegroundColor Red
}
