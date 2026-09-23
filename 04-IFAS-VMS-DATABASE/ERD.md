# IFAS VMS Logical ERD

```text
Roles
  |
  +----< RolePermissions >---- Permissions
  |
  +----< Users

Users
  |
  +---- AuditLogs

Cameras
  |
  +---- operational camera data

Licenses
  |
  +---- license limits / expiry / signature

Customers
  |
  +---- licenses
```

The exact physical relationship between the Server database and License Manager database is intentionally separated in the current architecture.

The License Manager owns customer and license issuance data.

The VMS Server owns runtime users, cameras, audit logs and the installed license state.
