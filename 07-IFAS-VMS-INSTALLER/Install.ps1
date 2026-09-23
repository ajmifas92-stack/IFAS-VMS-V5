#requires -Version 5.1
[CmdletBinding()] param([string]$SourceRoot=$PSScriptRoot,[string]$InstallRoot="$env:ProgramFiles\IFAS\IFAS VMS",[string]$DataRoot="$env:ProgramData\IFAS\IFAS VMS")
$ErrorActionPreference="Stop"
function Admin{$id=[Security.Principal.WindowsIdentity]::GetCurrent();$p=New-Object Security.Principal.WindowsPrincipal($id);if(!$p.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)){throw "Run IFAS VMS installer as Administrator."}}
function CopyTree($s,$d){if(!(Test-Path $s)){throw "Missing package component: $s"};New-Item -ItemType Directory -Force -Path $d|Out-Null;Copy-Item "$s\*" $d -Recurse -Force}
Admin
$pub=Join-Path $SourceRoot "Published"; if(!(Test-Path $pub)){throw "Published folder missing. Run Build-Publish.ps1 first."}
New-Item -ItemType Directory -Force -Path $InstallRoot,$DataRoot,(Join-Path $DataRoot 'Data'),(Join-Path $DataRoot 'Keys')|Out-Null
CopyTree (Join-Path $pub 'Server') (Join-Path $InstallRoot 'Server')
CopyTree (Join-Path $pub 'Client') (Join-Path $InstallRoot 'Client')
if(Test-Path (Join-Path $pub 'Updater')){CopyTree (Join-Path $pub 'Updater') (Join-Path $InstallRoot 'Updater')}
# Put writable server data beside ProgramData; generate config with absolute paths.
$server=Join-Path $InstallRoot 'Server\IFAS.Server.exe'
if(!(Test-Path $server)){throw "Server executable missing."}
& sc.exe stop 'IFAS VMS Server' | Out-Null 2>&1; & sc.exe delete 'IFAS VMS Server' | Out-Null 2>&1
& sc.exe create 'IFAS VMS Server' binPath= "`"$server`"" start= auto DisplayName= "IFAS VMS Server" | Out-Null
& sc.exe description 'IFAS VMS Server' 'IFAS VMS backend service' | Out-Null
$startMenu=Join-Path $env:ProgramData 'Microsoft\Windows\Start Menu\Programs\IFAS VMS';New-Item -ItemType Directory -Force -Path $startMenu|Out-Null
$shell=New-Object -ComObject WScript.Shell
$client=Join-Path $InstallRoot 'Client\IFAS.VMS.Client.exe';$lnk=$shell.CreateShortcut((Join-Path $startMenu 'IFAS VMS Client.lnk'));$lnk.TargetPath=$client;$lnk.WorkingDirectory=Split-Path $client;$lnk.Save()
$desk=Join-Path ([Environment]::GetFolderPath('CommonDesktopDirectory')) 'IFAS VMS Client.lnk';$lnk=$shell.CreateShortcut($desk);$lnk.TargetPath=$client;$lnk.WorkingDirectory=Split-Path $client;$lnk.Save()
& sc.exe start 'IFAS VMS Server' | Out-Null
Write-Host "IFAS VMS installed successfully." -ForegroundColor Green
Write-Host "Install: $InstallRoot";Write-Host "Data: $DataRoot"
