#requires -Version 5.1
[CmdletBinding()]
param([string]$InstallRoot="$env:ProgramFiles\IFAS\IFAS License Manager")
$ErrorActionPreference="Stop"
$identity=[Security.Principal.WindowsIdentity]::GetCurrent()
$principal=New-Object Security.Principal.WindowsPrincipal($identity)
if(-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)){throw "Run as Administrator."}
New-Item -ItemType Directory -Force -Path $InstallRoot | Out-Null
$source=Join-Path $PSScriptRoot "Published"
if(-not (Test-Path $source)){throw "Published supplier build not found. Build the supplier desktop project first."}
Copy-Item "$source\*" $InstallRoot -Recurse -Force
$exe=Join-Path $InstallRoot "IFAS.LicenseCreator.Desktop.exe"
if(Test-Path $exe){
  $start=Join-Path $env:ProgramData "Microsoft\Windows\Start Menu\Programs\IFAS VMS Supplier"
  New-Item -ItemType Directory -Force -Path $start | Out-Null
  $shell=New-Object -ComObject WScript.Shell
  $sc=$shell.CreateShortcut((Join-Path $start "IFAS License Creator.lnk"))
  $sc.TargetPath=$exe; $sc.WorkingDirectory=$InstallRoot; $sc.IconLocation="$exe,0"; $sc.Save()
}
Write-Host "IFAS License Creator installed for supplier use only." -ForegroundColor Green
