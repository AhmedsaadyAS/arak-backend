# ============================================================
# Arak Backend — Full Migration & Build Script
# Run from the REPOSITORY ROOT in a terminal where .NET 9 SDK
# is available (inside Cursor / VS Code integrated terminal).
# ============================================================

Write-Host "==> Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore Arak.sln

Write-Host ""
Write-Host "==> Building entire solution (DAL -> BLL -> PLL)..." -ForegroundColor Cyan
dotnet build Arak.sln --configuration Debug --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[ERROR] Build failed. Fix compilation errors before migrating." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "==> Creating migration 'CompleteCoreEntities'..." -ForegroundColor Cyan
Write-Host "    (includes Event, Fee, Evaluation, updated Student + Identity tables)" -ForegroundColor Gray
dotnet ef migrations add CompleteCoreEntities `
    --project Arak.DAL\Arak.DAL.csproj `
    --startup-project Arak.PLL\Arak.PLL.csproj `
    --output-dir Migrations

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[WARN] Migration might already exist. Trying database update directly..." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "==> Applying all pending migrations to SQL Server (ArakDB)..." -ForegroundColor Cyan
dotnet ef database update `
    --project Arak.DAL\Arak.DAL.csproj `
    --startup-project Arak.PLL\Arak.PLL.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[ERROR] Database update failed. Check SQL Server connection in appsettings.json." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "[SUCCESS] All done!" -ForegroundColor Green
Write-Host ""
Write-Host "New tables created:" -ForegroundColor Green
Write-Host "  ArakEvents, Fees, Evaluations (updated Student columns)" -ForegroundColor Green
Write-Host "  AspNetUsers, AspNetRoles, AspNetUserRoles (Identity)" -ForegroundColor Green
Write-Host ""
Write-Host "On first startup the app will auto-seed:" -ForegroundColor Green
Write-Host "  Roles: Super Admin, Admin, Academic Admin, Teacher, Fees Admin, Users Admin, Parent" -ForegroundColor Green
Write-Host "  User:  admin@arak.com / Admin@123  (role: Admin)" -ForegroundColor Green
Write-Host ""
Write-Host "Test login with Swagger UI at: https://localhost:7000/swagger" -ForegroundColor Cyan
Write-Host "  POST /api/auth/login  { email: 'admin@arak.com', password: 'Admin@123' }" -ForegroundColor Cyan
