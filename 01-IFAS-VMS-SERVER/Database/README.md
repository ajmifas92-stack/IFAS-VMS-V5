# IFAS VMS Server Database
Development uses SQLite and `EnsureCreatedAsync()` for the initial foundation. Production deployments should move to EF Core migrations and a managed database (for example PostgreSQL or SQL Server), with proper backups and secret handling.
