# Release Checklist

- [ ] Build all projects in Release configuration.
- [ ] Run automated tests.
- [ ] Verify database initialization/migrations.
- [ ] Verify license signature and server-side limits.
- [ ] Verify HTTPS/TLS certificate configuration.
- [ ] Verify JWT/secret configuration is externalized.
- [ ] Verify no private signing keys are included.
- [ ] Code-sign binaries.
- [ ] Build signed MSI/MSIX or enterprise installer.
- [ ] Generate SHA-256 release checksums.
- [ ] Test install, upgrade and uninstall on clean Windows.
- [ ] Test backup/restore before production rollout.
