[CmdletBinding()] param([string]$Configuration="Release")
$ErrorActionPreference="Stop"
$here=$PSScriptRoot
& (Join-Path $here 'Build-Publish.ps1') -Configuration $Configuration
if($LASTEXITCODE -ne 0){throw 'Publish failed'}
$root=Resolve-Path (Join-Path $here '..')
$stage=Join-Path $here 'StorePackage';Remove-Item $stage -Recurse -Force -ErrorAction SilentlyContinue;New-Item -ItemType Directory -Force -Path $stage|Out-Null
Copy-Item (Join-Path $here 'Published') (Join-Path $stage 'Published') -Recurse -Force
Copy-Item (Join-Path $here 'Install.ps1') $stage -Force
Copy-Item (Join-Path $here 'Preflight.ps1') $stage -Force
Copy-Item (Join-Path $here 'InstallerConfig.json') $stage -Force
$readme=Join-Path $stage 'INSTALL.txt';Set-Content $readme "IFAS VMS\r\n\r\n1. Open PowerShell as Administrator.\r\n2. Run: .\Install.ps1\r\n3. Start IFAS VMS Client from the desktop shortcut.\r\n\r\nSupplier License Creator is not included in the customer runtime package. It is published separately from Published\LicenseCreator."
$zip=Join-Path $here 'IFAS-VMS-STORE-PACKAGE.zip';Remove-Item $zip -Force -ErrorAction SilentlyContinue;Compress-Archive -Path (Join-Path $stage '*') -DestinationPath $zip -Force
Write-Host "STORE PACKAGE: $zip" -ForegroundColor Green
