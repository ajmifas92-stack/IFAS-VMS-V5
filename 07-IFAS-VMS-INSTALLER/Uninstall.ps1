#requires -Version 5.1
[CmdletBinding()]
param(
    [string]$InstallRoot = "$env:ProgramFiles\IFAS\IFAS VMS",
    [string]$DataRoot = "$env:ProgramData\IFAS\IFAS VMS",
    [switch]$RemoveData
)

$ErrorActionPreference = "Stop"

$identity = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = New-Object Security.Principal.WindowsPrincipal($identity)
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw "Please run the uninstaller as Administrator."
}

$startMenu = Join-Path $env:ProgramData "Microsoft\Windows\Start Menu\Programs\IFAS VMS"
if (Test-Path $startMenu) {
    Remove-Item $startMenu -Recurse -Force
}

if (Test-Path $InstallRoot) {
    Remove-Item $InstallRoot -Recurse -Force
}

if ($RemoveData -and (Test-Path $DataRoot)) {
    Remove-Item $DataRoot -Recurse -Force
    Write-Host "Application and data removed."
}
else {
    Write-Host "Application removed. Data was retained at: $DataRoot"
}
