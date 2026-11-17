# wpfw-portfolio-api

- .NET-versie: 
**10**
- API-stijl en structuur: 
**Minimal API, per feature (Auth, Projects en BlogPosts)**
- SQL Server: 
**SQL Server Express, connectionstring uit appsettings.json**
- Swagger: **http://localhost:5080/swagger**

## Commandos

1. Migraties: `dotnet ef database update`
2. Starten: `dotnet run`
3. Testen: `dotnet test`

## Gebruikers en rollen

- Gast: registreer via `POST /api/auth/register`.
- Editor of Admin: voer `scripts/rol-toekennen.sql` uit op de database met de juiste variablen en vervolgens opnieuwe inloggen.