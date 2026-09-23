[CmdletBinding()]
param([string]$Configuration="Release",[string]$OutputRoot="$PSScriptRoot\Published")
$ErrorActionPreference="Stop"
$repo=Resolve-Path (Join-Path $PSScriptRoot "..")
$projects=@{Server="01-IFAS-VMS-SERVER\IFAS.Server.csproj";Client="03-IFAS-VMS-CLIENT\IFAS.VMS.Client.csproj";Updater="08-IFAS-VMS-UPDATER\IFAS.VMS.Updater.csproj";LicenseCreator="02-IFAS-LICENSE-MANAGER\IFAS.LicenseCreator.Desktop\IFAS.LicenseCreator.Desktop.csproj"}
Remove-Item $OutputRoot -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null
foreach($name in $projects.Keys){$project=Join-Path $repo $projects[$name];$dest=Join-Path $OutputRoot $name;Write-Host "Publishing $name..." -ForegroundColor Cyan;dotnet publish $project -c $Configuration -r win-x64 --self-contained true -p:PublishSingleFile=false -o $dest;if($LASTEXITCODE -ne 0){throw "Publish failed for $name"}}
# Customer runtime excludes supplier LicenseCreator.
Write-Host "Publish complete. Customer runtime is Server + Client + Updater." -ForegroundColor Green
