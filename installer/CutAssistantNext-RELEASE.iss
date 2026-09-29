; Local release candidate; compile from a verified publish directory.
#ifndef CanPublishDir
  #error CanPublishDir is required
#endif
#ifndef CanOutputDir
  #error CanOutputDir is required
#endif
#ifndef CanVersion
  #error CanVersion is required
#endif
#ifndef CanBuildNumber
  #error CanBuildNumber is required
#endif
#ifndef CanCandidate
  #error CanCandidate is required
#endif

[Setup]
AppId={{cdcdaf11-d631-4f81-b1d0-b50ea6ac606d}
AppName=Cut Assistant Next
AppVersion={#CanVersion}
AppVerName=Cut Assistant Next {#CanVersion} (Build {#CanBuildNumber}, {#CanCandidate})
DefaultDirName={autopf}\Cut Assistant Next
DefaultGroupName=Cut Assistant Next
SetupArchitecture=x64
PrivilegesRequired=admin
WizardStyle=modern dynamic
WizardSizePercent=150,150
SetupIconFile=..\assets\can.ico
OutputDir={#CanOutputDir}
OutputBaseFilename=CutAssistantNext-{#CanVersion}-Build{#CanBuildNumber}-win-x64-Setup-{#CanCandidate}
InfoAfterFile={#CanPublishDir}\VORAUSSETZUNGEN.txt
Compression=lzma2
SolidCompression=yes
UninstallDisplayIcon={app}\CutAssistantNext.App.exe

[Languages]
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Files]
Source: "{#CanPublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Cut Assistant Next"; Filename: "{app}\CutAssistantNext.App.exe"; WorkingDir: "{app}"
Name: "{autodesktop}\Cut Assistant Next"; Filename: "{app}\CutAssistantNext.App.exe"; WorkingDir: "{app}"
