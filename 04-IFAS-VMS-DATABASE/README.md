# 04-IFAS-VMS-DATABASE

This module defines the shared database foundation for IFAS VMS.

Current architecture:
- SQLite is used for local development.
- The server and License Manager currently own their operational databases.
- This package provides the planned shared database structure, SQL schema, seed data and documentation.
- Production deployment can move to PostgreSQL or SQL Server later without changing the logical data model.

Important:
Do not manually create database files from these SQL scripts when running the existing Server or License Manager projects. Their application startup code creates their own SQLite databases automatically.

Folders:
- Schema/       Database table definitions
- Seeds/        Initial reference data
- Migrations/   Versioning notes
- Backup/       Backup/restore instructions
