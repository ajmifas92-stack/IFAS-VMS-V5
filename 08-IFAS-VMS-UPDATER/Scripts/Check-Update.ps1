[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$ManifestUrl,

    [Parameter(Mandatory=$true)]
    [string]$CurrentVersion
)

$ErrorActionPreference = "Stop"

if ($ManifestUrl -notmatch '^https://') {
    throw "Update manifest must use HTTPS."
}

$manifest = Invoke-RestMethod -Uri $ManifestUrl -Method Get

Write-Host "Current version: $CurrentVersion"
Write-Host "Available version: $($manifest.version)"
Write-Host "Mandatory: $($manifest.mandatory)"
