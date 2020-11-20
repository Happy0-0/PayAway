List current global tools
dotnet tool list -g

Install VS Extension "EF Power Tools"

------------------------------------------
Entity FRamework Reference
------------------------------------------
dotnet tool install --global dotnet-ef --version 5.0.0-rc.2.20475.6
add Microsoft.EntityFrameworkCore.Design -Version 5.0.0-rc.2.20475.6
add Microsoft.EntityFrameworkCore.Sqlite -Version 5.0.0-rc.2.20475.6
remove Microsoft.EntityFrameworkCore.SqlServer

dotnet ef migrations add CreateDatabase
dotnet ef database update

------------------------------------------
SQLite Reference
------------------------------------------
DB Browser for SQLite
	https://sqlitebrowser.org/

https://www.sqlite.org/autoinc.html

------------------------------------------
Tech Debt Items
------------------------------------------
1.	Chg JsonAttribute of *Id properties in MBEs to *Guid	<===== Will require Front End refactoring
2.	<comp> Validate phone nos on the way in, store normalized format in the db
3.	<comp> Convert DT DB values to GMT
4.	**Update from .net 5 rc2 to rtm
5.	**Update nuget packages
6.	**Update Publish to Azure App Service to remove Self-Contained
7.	Cleanup Code Analysis messages
8.	<comp> Add Table Relationships via navigation properties
9.	Convert to use Record types?
10. Refactor OrderHeaderMBE
11. <wip> Implement SMS Send
12. Implement Image Upload
13. Refactor OrderNumber vs OrderId
14. Always restore default order items