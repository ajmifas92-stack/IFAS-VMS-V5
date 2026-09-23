[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$PackageFile,

    [Parameter(Mandatory=$true)]
    [string]$ExpectedSha256,

    [Parameter(Mandatory=$true)]
    [string]$InstallRoot
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $PackageFile)) {
    throw "Package not found: $PackageFile"
}

$actual = (Get-FileHash -Algorithm SHA256 -Path $PackageFile).Hash.ToLowerInvariant()
if ($actual -ne $ExpectedSha256.ToLowerInvariant()) {
    throw "SHA-256 verification failed."
}

$timestamp = Get-Date -Format "yyyyMMddHHmmss"
$backup = "$InstallRoot.backup-$timestamp"
$staging = Join-Path $env:TEMP "IFAS-VMS-Update-$timestamp"

New-Item -ItemType Directory -Force -Path $staging | Out-Null
Expand-Archive -Path $PackageFile -DestinationPath $staging -Force

if (Test-Path $InstallRoot) {
    Move-Item -Path $InstallRoot -Destination $backup
}

try {
    Move-Item -Path $staging -Destination $InstallRoot
    Write-Host "Update installed successfully."
}
catch {
    if (-not (Test-Path $InstallRoot) -and (Test-Path $backup)) {
        Move-Item -Path $backup -Destination $InstallRoot
    }
    throw
}
