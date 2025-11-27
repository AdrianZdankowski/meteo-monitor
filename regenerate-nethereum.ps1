# Regenerate Nethereum C# classes from SensorToken ABI

Write-Host "Regenerating Nethereum classes from SensorToken ABI..." -ForegroundColor Yellow

# Check if tool is installed
$toolInstalled = $null -ne (Get-Command "Nethereum.Generator.Console" -ErrorAction SilentlyContinue)

if (-not $toolInstalled) {
    Write-Host "Installing Nethereum.Generator.Console..." -ForegroundColor Cyan
    dotnet tool install -g Nethereum.Generator.Console
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to install Nethereum.Generator.Console" -ForegroundColor Red
        exit 1
    }
}

# Paths
$backendDir = Join-Path $PSScriptRoot "backend"
$abiPath = Join-Path $PSScriptRoot "blockchain\SensorToken.abi.json"

# Check if ABI exists
if (-not (Test-Path $abiPath)) {
    Write-Host "Error: ABI file not found at $abiPath" -ForegroundColor Red
    Write-Host "Please deploy the contract first: cd blockchain && npm run deploy" -ForegroundColor Yellow
    exit 1
}

Write-Host "Using ABI: $abiPath" -ForegroundColor Gray
Write-Host "Output: $backendDir\Services" -ForegroundColor Gray

# Change to backend directory
Push-Location $backendDir

try {
    # Generate C# classes
    Nethereum.Generator.Console generate from-abi -abi "..\blockchain\SensorToken.abi.json" -bin "..\blockchain\SensorToken.bin" -o "Services" -ns "backend.Services.ContractDefinition" -cn "SensorContract"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "Successfully generated files:" -ForegroundColor Green
        Write-Host "  - Services\SensDefinition.gen.cs" -ForegroundColor Green
        Write-Host "  - Services\SensService.cs" -ForegroundColor Green
        
        # Show file timestamps
        $genFiles = Get-ChildItem "Services\*.cs"
        Write-Host ""
        Write-Host "File timestamps:" -ForegroundColor Gray
        foreach ($file in $genFiles) {
            Write-Host "  $($file.Name): $($file.LastWriteTime)" -ForegroundColor Gray
        }
    } else {
        Write-Host "Error: Code generation failed" -ForegroundColor Red
        exit 1
    }
} finally {
    Pop-Location
}

Write-Host ""
Write-Host "Done!" -ForegroundColor Green
