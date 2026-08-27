param(
    [string]$Password = "ZAQwsxCDE@91"
)

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host " Building .NET Backend API (UAT)..." -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

Set-Location $PSScriptRoot
dotnet publish LIMSApi/LIMSApi.csproj -c Release -o LIMSApi/bin/Release/net8.0/publish

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

# Copy appsettings.Secrets.json to publish directory to ensure live connection strings are present
$SecretsSrc = Join-Path $PSScriptRoot "LIMSApi\appsettings.Secrets.json"
$SecretsDst = Join-Path $PSScriptRoot "LIMSApi\bin\Release\net8.0\publish\appsettings.Secrets.json"
if (Test-Path $SecretsSrc) {
    Copy-Item $SecretsSrc $SecretsDst -Force
}

Write-Host "==========================================" -ForegroundColor Green
Write-Host " Deploying to IIS UAT Server..." -ForegroundColor Green
Write-Host " Site: dmspl91-001-site6" -ForegroundColor Green
Write-Host " URL: http://dmspl91-001-site6.htempurl.com/" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green

$MSDeployPath = "C:\Program Files (x86)\IIS\Microsoft Web Deploy V3\msdeploy.exe"
if (-not (Test-Path $MSDeployPath)) {
    $MSDeployPath = "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe"
}

$SourcePath = Join-Path $PSScriptRoot "LIMSApi\bin\Release\net8.0\publish"

& $MSDeployPath -verb:sync `
    -source:contentPath="$SourcePath" `
    -dest:contentPath="dmspl91-001-site6",wmsvc="https://win6046.site4now.net:8172/MsDeploy.axd?site=dmspl91-001-site6",userName="dmspl91-001",password="$Password",authtype="Basic" `
    -enableRule:AppOffline `
    -allowUntrusted

if ($LASTEXITCODE -eq 0) {
    Write-Host "==========================================" -ForegroundColor Green
    Write-Host " SUCCESS! Backend API deployed to UAT (Site-6)!" -ForegroundColor Green
    Write-Host " Live URL: http://dmspl91-001-site6.htempurl.com/" -ForegroundColor Green
    Write-Host "==========================================" -ForegroundColor Green
} else {
    Write-Host "Deployment failed with error code $LASTEXITCODE" -ForegroundColor Red
}
