dotnet ef migrations remove --force --project "./DP"
dotnet ef migrations add migration --project "./DP"
dotnet ef database update --project "./DP"

@echo off

echo.
echo.
echo ----------------- Done -----------------
pause