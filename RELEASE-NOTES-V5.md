# IFAS VMS V5 - Windows Package

This checkpoint prepares the IFAS VMS solution for Windows x64 build/publish and one-click installation.

## Important
This environment cannot run the Windows .NET build toolchain, so this package includes Windows validation/build scripts. The package must be built on a Windows runner before a binary installer can be claimed as tested.

## Main fixes
- Clean solution/project structure.
- Explicit project references.
- Correct WPF/WinForms `Application` ambiguity handling.
- Project-local application icons.
- Self-contained win-x64 publish.
- Server Windows Service support.
- One-click build/package/install scripts.
- Preflight and smoke-test scripts.
- Separate supplier License Creator from customer runtime package.
- No supplier private signing key is shipped to customers.
