dotnet tool install --global dotnet-ef --version 5.0.0-rc.2.20475.6
add Microsoft.EntityFrameworkCore.Design -Version 5.0.0-rc.2.20475.6
add Microsoft.EntityFrameworkCore.Sqlite -Version 5.0.0-rc.2.20475.6
remove Microsoft.EntityFrameworkCore.SqlServer

dotnet ef migrations add CreateDatabase
dotnet ef database update