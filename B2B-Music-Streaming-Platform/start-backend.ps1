$Host.UI.RawUI.WindowTitle = "Backend API Server"
Set-Location "C:\Projects\B2B-Frontend_JP\B2B-Music-Streaming-Platform\B2BMusicStreamingPlatformwebapi"
Write-Host "Starting Backend API Server..." -ForegroundColor Cyan
dotnet run
