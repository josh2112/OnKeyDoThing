﻿; -- OnKeyDoThing Install Script --

#define Application   "OnKeyDoThing"

#define Exe           "OnKeyDoThing.exe"
#define ExeDir        "bin\Release\net6.0-windows"
#define ExeVersion    GetFileVersion( AddBackslash( ExeDir ) + Exe )

#define Company       "Josh2112 Apps"

[Setup]
AppName={#Application}
AppVerName={#Application}
AppVersion={#ExeVersion}
AppPublisher={#Company}
AppPublisherURL=https://www.josh2112.com/apps/
AppCopyright=Copyright (C) 2021 {#Company}
VersionInfoVersion={#ExeVersion}
DefaultDirName={commonpf}\{#Company}\{#Application}
DefaultGroupName={#Company}
MinVersion=6.1.7601
Compression=lzma2
SolidCompression=yes
OutputBaseFilename="{#Application} Installer - {#ExeVersion}"
DirExistsWarning=no
DisableDirPage=yes
DisableProgramGroupPage=yes
UninstallDisplayIcon="{app}\{#Exe}"

[Files]
Source: "{#ExeDir}\*"; Excludes: "*.pdb,*.xml,*.vshost.*"; DestDir: "{app}"; Flags: replacesameversion recursesubdirs

[Icons]
Name: "{group}\{#Application}"; Filename: "{app}\{#Exe}"
Name: "{commondesktop}\{#Application}"; Filename: "{app}\{#Exe}"

[Run]
Filename: "{app}\{#Exe}"; Description: "Run {#Application}"; Flags: postinstall skipifsilent nowait
