# IFAS VMS Server

.NET 8 ASP.NET Core backend foundation for IFAS VMS.

## Includes
- JWT authentication
- PBKDF2 password hashing
- Users and roles
- Camera/device registry
- License installation and server-side limit checking
- Audit service foundation
- Security headers and exception handling
- Swagger API documentation
- SQLite development database

## Development login
Username: `admin`
Password: `admin`

Change this immediately in any real deployment.

## Important production work
- Replace the development JWT secret.
- Protect the license public key path and deploy the correct public key.
- Encrypt camera credentials at rest.
- Use HTTPS/TLS behind a proper reverse proxy or Kestrel HTTPS configuration.
- Move from `EnsureCreatedAsync()` to EF Core migrations.
- Add full ONVIF/RTSP media pipeline, recording, playback and PTZ services.
- Add rate limiting and stronger operational logging/monitoring.
