# IFAS VMS Windows build/install

On Windows 10/11 x64 with .NET 8 SDK installed:

1. Run `07-IFAS-VMS-INSTALLER\Build-StorePackage.cmd`.
2. It restores, builds, publishes self-contained win-x64 components, and creates `IFAS-VMS-STORE-PACKAGE.zip`.
3. On a clean test PC, extract the ZIP.
4. Run PowerShell as Administrator and run `Install.ps1`.
5. Start `IFAS VMS Client` from the desktop.
6. The server is registered as `IFAS VMS Server`.
7. Run `Smoke-Test.ps1` after installation/build to validate the server port.

The supplier License Creator is deliberately not part of the customer runtime installer because its private signing key is security-sensitive.
