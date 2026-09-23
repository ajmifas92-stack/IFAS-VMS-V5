[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"

Write-Host "IFAS VMS Installer Preflight" -ForegroundColor Cyan
Write-Host "OS: $([System.Environment]::OSVersion.VersionString)"
Write-Host "Architecture: $env:PROCESSOR_ARCHITECTURE"

$os = Get-CimInstance Win32_OperatingSystem
Write-Host "Windows: $($os.Caption)"

$free = (Get-PSDrive -Name C).Free / 1GB
Write-Host ("Free C: {0:N1} GB" -f $free)

if ($free -lt 2) {
    Write-Warning "Less than 2 GB free disk space is available."
}

Write-Host "Preflight complete."
