#define AppName "IFAS VMS"
#define AppVersion "5.0"
#define Publisher "IFAS"
#define InstallDir "{autopf}\IFAS\IFAS VMS"
#define DataDir "{commonappdata}\IFAS\IFAS VMS"

[Setup]
AppId={{IFAS-VMS-5}}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#Publisher}
DefaultDirName={#InstallDir}
DefaultGroupName=IFAS VMS
OutputDir=..\Release
OutputBaseFilename=IFAS-VMS-Setup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayName=IFAS VMS

[Files]
Source: "..\Published\Server\*"; DestDir: "{app}\Server"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Published\Client\*"; DestDir: "{app}\Client"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Published\Updater\*"; DestDir: "{app}\Updater"; Flags: ignoreversion recursesubdirs createallsubdirs

[Dirs]
Name: "{#DataDir}\Data"
Name: "{#DataDir}\Keys"

[Icons]
Name: "{group}\IFAS VMS Client"; Filename: "{app}\Client\IFAS.VMS.Client.exe"; WorkingDir: "{app}\Client"
Name: "{commondesktop}\IFAS VMS Client"; Filename: "{app}\Client\IFAS.VMS.Client.exe"; WorkingDir: "{app}\Client"

[Run]
Filename: "{sys}\sc.exe"; Parameters: "stop ""IFAS VMS Server"""; Flags: runhidden waituntilterminated
Filename: "{sys}\sc.exe"; Parameters: "delete ""IFAS VMS Server"""; Flags: runhidden waituntilterminated
Filename: "{sys}\sc.exe"; Parameters: "create ""IFAS VMS Server"" binPath= ""{app}\Server\IFAS.Server.exe"" start= auto DisplayName= ""IFAS VMS Server"""; Flags: runhidden waituntilterminated
Filename: "{sys}\sc.exe"; Parameters: "description ""IFAS VMS Server"" ""IFAS VMS backend service"""; Flags: runhidden waituntilterminated
Filename: "{sys}\sc.exe"; Parameters: "start ""IFAS VMS Server"""; Flags: runhidden waituntilterminated
