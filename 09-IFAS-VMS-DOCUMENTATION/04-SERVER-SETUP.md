# Server Setup

## Configuration areas

- Server host/port
- HTTPS/TLS
- Database connection
- JWT secret
- License verification public key
- Logging
- Camera settings

## Security

Never use development secrets in production.

Store:
- JWT secrets
- database credentials
- TLS private keys
- license-management secrets

outside source control and preferably in an OS/enterprise secret store.

## Verification checklist

- Server starts successfully.
- HTTPS endpoint is reachable.
- Authentication works.
- License status is valid.
- Database initializes/migrates correctly.
- Audit events are written.
