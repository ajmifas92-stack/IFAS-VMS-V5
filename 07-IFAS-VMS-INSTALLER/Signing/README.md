# Installer Signing

Production releases should be code-signed.

Recommended release checks:
- Sign application EXEs/DLLs.
- Sign installer/MSI/MSIX.
- Verify signatures before release.
- Keep the signing certificate private.
- Maintain a release manifest/checksum.
- Do not distribute private signing keys.
