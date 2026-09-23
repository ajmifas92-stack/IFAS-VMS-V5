[CmdletBinding()] param([int]$Port=5080)
$ErrorActionPreference='Stop'
$checks=@()
$server=Join-Path $PSScriptRoot 'Published\Server\IFAS.Server.exe';$client=Join-Path $PSScriptRoot 'Published\Client\IFAS.VMS.Client.exe'
$checks += [pscustomobject]@{Check='Server executable';OK=(Test-Path $server)}
$checks += [pscustomobject]@{Check='Client executable';OK=(Test-Path $client)}
$checks += [pscustomobject]@{Check='Updater executable';OK=(Test-Path (Join-Path $PSScriptRoot 'Published\Updater\IFAS.VMS.Updater.exe'))}
$tcp=Test-NetConnection 127.0.0.1 -Port $Port -WarningAction SilentlyContinue; $checks += [pscustomobject]@{Check="Server port $Port";OK=$tcp.TcpTestSucceeded}
$checks|Format-Table -AutoSize
if($checks.OK -contains $false){exit 1}
