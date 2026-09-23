# IFAS VMS Database Migrations

The current Server and License Manager projects use Entity Framework Core with SQLite and call `EnsureCreatedAsync()` during development startup.

This folder documents the planned migration approach.

## Development

For the current prototype:
- Database creation is automatic.
- Do not manually run the SQL schema against the application's live database unless you intentionally create a separate database.

## Production

Before commercial production release:
1. Replace `EnsureCreatedAsync()` with EF Core migrations.
2. Create an explicit migration history.
3. Test upgrades from every supported previous version.
4. Back up the database before upgrades.
5. Provide rollback/recovery procedures.
6. Restrict database file permissions.

Recommended commands when EF migrations are introduced:

```text
dotnet ef migrations add InitialCreate
dotnet ef database update
```

The exact migration commands depend on which project owns the production database.
