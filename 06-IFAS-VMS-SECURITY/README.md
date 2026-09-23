# 06-IFAS-VMS-SECURITY

Shared security foundation for IFAS VMS.

Includes:
- Password hashing with PBKDF2
- JWT token helper
- API key generation
- Cryptographic utility helpers
- Security policy constants
- Secure random token generation

Target: .NET 8

Important:
This is a development/security foundation. Production deployment should use managed secrets, HTTPS/TLS, key rotation, secure key storage, rate limiting, monitoring and a formal security review.
