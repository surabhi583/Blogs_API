# [.NET CORE 8.0 Blogs Example Api]

> ### [Dotnet 8] codebase containing Technical Article Blogs examples (CRUD, auth, advanced patterns, etc) 

This codebase was created to demonstrate a fully fledged fullstack application built with **[Dotnet 8]** including CRUD operations, authentication, routing, pagination, and more.

We've gone to great lengths to adhere to the **[Dotnet 8]** community styleguides & best practices.

For more information on how to this works with frontends, head over to the https://github.com/surabhi583/Blogs_UI repo.

# How it works

Traditional Clean Architecture setup using Dotnet 6.
Consisting of the following layers:

- api
- core
- data
- infrastructure

Build using the following features:

- the Startup class
- file scoped namespaces
- Entity Framework Core with SQL Server db
- serilog for file and console logging

## migrations

Add migration by going to Data folder and execute:
dotnet ef migrations add MigrationName --startup-project ../Api/Api.csproj

Run db upgrade: run command : Database Update in package console of Visual studio.
dotnet ef database update --startup-project ../Api/Api.csproj
