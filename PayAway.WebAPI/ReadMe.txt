List current global tools
dotnet tool list -g

Install VS Extension "EF Power Tools"

------------------------------------------
HTTP Status Codes
------------------------------------------
https://www.restapitutorial.com/httpstatuscodes.html

------------------------------------------
Problem Details
------------------------------------------
https://andrewlock.net/handling-web-api-exceptions-with-problemdetails-middleware/

------------------------------------------
SignalR
------------------------------------------
https://blog.ctglobalservices.com/scripting-development/mme/send-instant-message-from-server-back-to-client-using-signalr/
https://procodeguide.com/programming/real-time-web-with-signalr-in-aspnet-core/
https://code-maze.com/netcore-signalr-angular/

------------------------------------------
Record Type
------------------------------------------
https://www.claudiobernasconi.ch/2020/11/07/csharp-9-record-types-introduction-and-deep-dive/

------------------------------------------
Entity Framework Reference
------------------------------------------
dotnet tool install --global dotnet-ef --version 5.0.0-rc.2.20475.6
add Microsoft.EntityFrameworkCore.Design -Version 5.0.0-rc.2.20475.6
add Microsoft.EntityFrameworkCore.Sqlite -Version 5.0.0-rc.2.20475.6
remove Microsoft.EntityFrameworkCore.SqlServer

dotnet ef migrations add <name> --project PayAway.WebAPI
dotnet ef database update --project PayAway.WebAPI

------------------------------------------
SQLite Reference
------------------------------------------
DB Browser for SQLite
	https://sqlitebrowser.org/

https://www.sqlite.org/autoinc.html

------------------------------------------
Tech Debt Items
------------------------------------------
10. Refactor OrderHeaderMBE (on hold)
-------------------------------------------------------------------------------
    <tom> 
-------------------------------------------------------------------------------
	<gabe>
-------------------------------------------------------------------------------
1.	<comp> Rename JsonAttribute of *Id properties in MBEs to *Guid			<===== Will require Front End refactoring
2.	<comp> Validate phone nos on the way in, store normalized format in the db
3.	<comp> Convert Datetime DB values to GMT
4.	<comp> Update from .net 5 rc2 to rtm
5.	<comp> Update nuget packages
6.	<comp> Update Publish to Azure App Service to remove Self-Contained
7.	<comp> Cleanup Code Analysis messages
8.	<comp> Add Table Relationships via navigation properties
9.	<comp> Convert to use Record types on some MBEs
11. <comp> Implement SMS Send
12. <comp> Implement Image Upload
13. <comp> Refactor OrderNumber vs OrderId
14. <comp> Always restore default catalog items on ResetDB
15. <comp> Correct issue with methods not returning ProblemDetails
16. <comp> Rename all JsonAttribute *Date properties to *DateTimeUTC		<===== Will require Front End refactoring
17. <comp> Add flags to order												<===== Will need Front End changes
18. <comp> implemented WebHook (SignalR)
19. <comp> moved DB connection string out of source code
20. <comp> implemented best practice for managing lifetime of DBContext object