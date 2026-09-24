#define AppName "IFAS License Creator"
#define AppVersion "5.0"
#define Publisher "IFAS"
#define InstallDir "{autopf}\IFAS\License Creator"

[Setup]
AppId={{IFAS-LICENSE-CREATOR-5}}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#Publisher}
DefaultDirName={#InstallDir}
DefaultGroupName=IFAS License Creator
OutputDir=..\Release
OutputBaseFilename=IFAS-License-Creator-Setup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayName=IFAS License Creator
SetupIconFile=..\..\02-IFAS-LICENSE-MANAGER\IFAS.LicenseCreator.Desktop\Assets\IFAS-License-Manager.ico

[Files]
Source: "..\Published\LicenseCreator\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\IFAS License Creator"; Filename: "{app}\IFAS.LicenseCreator.exe"
Name: "{commondesktop}\IFAS License Creator"; Filename: "{app}\IFAS.LicenseCreator.exe"

[Run]
Filename: "{app}\IFAS.LicenseCreator.exe"; Description: "Launch IFAS License Creator"; Flags: postinstall nowait skipifsilent
