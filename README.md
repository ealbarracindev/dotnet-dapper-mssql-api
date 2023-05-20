# dotnet-7-dapper-mssql-crud-api

.NET 7.0 + Dapper + MS SQL Server - CRUD API Tutorial in ASP.NET Core

## Standalone
1. You may need a running instance of MsSQL, with appropriate migrations initialized.
	- You can run just the DB on docker. For that, you have to change your connection string to "Server=127.0.0.1;Database=master;User=sa;Password=Yourpassword123” and run the following command: ``docker-compose up -d db-server``. Doing that, the application will be able to reach de container of the db server.
	- If you want, you can change the DatabaseExtension to use UseInMemoryDatabase, instead of Mssql.
2. Go to the src/Boilerplate.Api folder and run ``dotnet run``, or, in visual studio set the api project as startup and run as console or docker (not IIS).
3. Visit http://localhost:5000/api-docs or https://localhost:5001/api-docs to access the application's swagger.


# This project contains:
- [x] SwaggerUI
- [x] Dapper
- [x] Repository
- [x] Serilog with request logging and easily configurable sinks
- [x] .Net Dependency Injection
- [x] Database initialization
- [x] Health Checks api and dependencies
- [x] Response compression
- [x] Rate limit
- [ ] Unit tests
- [ ] Integration tests