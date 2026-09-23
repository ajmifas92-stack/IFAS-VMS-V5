# Update Security

Production updater controls:
1. HTTPS-only manifest and package transport.
2. SHA-256 integrity verification.
3. Digital signature verification using a trusted public key.
4. Reject unsigned or invalid packages.
5. Verify package product and version.
6. Prevent unauthorized downgrade.
7. Back up the current installation.
8. Roll back if installation fails.
9. Keep update signing private keys off client machines.
10. Log update attempts and results.

The example manifest uses placeholders and is not a production trust configuration.
