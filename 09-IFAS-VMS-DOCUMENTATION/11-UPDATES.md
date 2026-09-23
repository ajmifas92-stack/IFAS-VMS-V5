# Updates

IFAS VMS includes an updater foundation.

Recommended release flow:

1. Build release.
2. Test.
3. Create package.
4. Calculate SHA-256.
5. Sign release/package metadata.
6. Publish over HTTPS.
7. Client checks update metadata.
8. Verify package.
9. Back up current installation.
10. Apply update.
11. Verify startup.
12. Roll back if required.

Never put update signing private keys on customer machines.
