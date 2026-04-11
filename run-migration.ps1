# ============================================================
# Arak Backend — Step 4: Database Migration Script
# Run this from the REPOSITORY ROOT in a terminal that has
# the .NET 9 SDK available (e.g. inside Cursor/VS Code terminal).
# ============================================================

Write-Host "==> Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore Arak.sln

Write-Host ""
Write-Host "==> Building solution to verify no compilation errors..." -ForegroundColor Cyan
dotnet build Arak.sln --configuration Debug

if ($LASTEXITCODE -ne 0) {
    Write-Host "" 
    Write-Host "[ERROR] Build failed. Fix compilation errors before running migration." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "==> Adding EF Core migration: 'CompleteCoreEntities'..." -ForegroundColor Cyan
dotnet ef migrations add CompleteCoreEntities `
    --project Arak.DAL\Arak.DAL.csproj `
    --startup-project Arak.PLL\Arak.PLL.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[ERROR] Migration creation failed." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "==> Applying migration to SQL Server (ArakDB)..." -ForegroundColor Cyan
dotnet ef database update `
    --project Arak.DAL\Arak.DAL.csproj `
    --startup-project Arak.PLL\Arak.PLL.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[ERROR] Database update failed. Check your SQL Server connection." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "[SUCCESS] Migration applied successfully!" -ForegroundColor Green
Write-Host "Tables created/updated: ArakEvents, Fees, Evaluations, Students (with new columns)" -ForegroundColor Green
