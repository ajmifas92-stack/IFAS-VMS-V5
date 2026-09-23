# 07-IFAS-VMS-INSTALLER

Windows installation package foundation for IFAS VMS.

This module contains installer configuration/documentation and a PowerShell bootstrap installer.
It is designed so the final Windows installer can package:
- IFAS VMS Server
- IFAS VMS Client
- IFAS License Manager
- required configuration/data folders

Target environment:
- Windows 10/11 64-bit
- .NET 8 runtime/SDK as required by the selected deployment mode

Important:
The included PowerShell script is a development/bootstrap installer, not a signed MSI.
For production distribution, package the published applications with a signed MSI/MSIX or another enterprise installer technology and code-sign the binaries.

## Supplier License Creator separation
The standard client/server installer does not install the License Creator. The supplier-only tool is installed separately from `02-IFAS-LICENSE-MANAGER/Supplier-Install.ps1`. Keep the private signing key and supplier build on a protected supplier workstation.
