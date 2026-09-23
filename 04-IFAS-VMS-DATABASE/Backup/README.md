# IFAS VMS Database Backup

## SQLite development backup

Stop the application before copying the SQLite database file.

Example files:

```text
01-IFAS-VMS-SERVER/Data/ifas.db
02-IFAS-LICENSE-MANAGER/Data/ifas-license-manager.db
```

Copy the database file to a protected backup location.

## Production

A production backup plan should include:
- Scheduled backups
- Multiple backup generations
- Offline or separate backup storage
- Encryption at rest
- Restore testing
- Access control
- Backup monitoring

Never publish database backups in a public repository.

## Recovery

Before restoring:
1. Stop the affected application.
2. Preserve the current database as an emergency copy.
3. Restore the verified backup.
4. Start the application.
5. Verify users, cameras, licenses and audit records.
6. Record the recovery event.
