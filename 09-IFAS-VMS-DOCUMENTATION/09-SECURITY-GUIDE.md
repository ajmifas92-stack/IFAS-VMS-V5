# Security Guide

## Required production controls

- HTTPS/TLS
- Strong password policy
- PBKDF2 password hashing
- JWT secret stored securely
- Server-side authorization
- License signature verification
- Audit logging
- Rate limiting
- Input validation
- Secure headers
- Secret/key rotation
- OS patching
- Database backup
- Signed releases
- Update package verification

## Network

Do not expose administration endpoints directly to the public Internet unless a properly secured architecture requires it.

Prefer:
- Firewall restrictions
- VPN/private network
- TLS
- Restricted management access

## Important

No software can honestly guarantee absolute or 100% security. Security is a continuous process involving code, infrastructure, configuration, operations and monitoring.
