; Cut Assistant Next – internes Test-Setup
; Nicht zur oeffentlichen Weitergabe freigegeben.

[Setup]
AppId={{cdcdaf11-d631-4f81-b1d0-b50ea6ac606d}
AppName=Cut Assistant Next
AppVersion=0.2.0
AppVerName=Cut Assistant Next 0.2.0 (Build 6)

DefaultDirName={autopf}\Cut Assistant Next
DefaultGroupName=Cut Assistant Next

SetupArchitecture=x64
PrivilegesRequired=admin
WizardStyle=modern dynamic
SetupIconFile=..\assets\can.ico

OutputDir=..\artifacts\installer-output
OutputBaseFilename=CutAssistantNext-0.2.0-Build6-setup-TEST5

InfoAfterFile=HINWEIS-EXTERNE-WERKZEUGE.txt

Compression=lzma2
SolidCompression=yes

UninstallDisplayIcon={app}\CutAssistantNext.App.exe

[Languages]
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Files]
Source: "..\artifacts\publish-kiss-build6-TEST4\*"; DestDir: "{app}"; Excludes: "\tools\ffmpeg\*"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Cut Assistant Next"; Filename: "{app}\CutAssistantNext.App.exe"; WorkingDir: "{app}"
Name: "{autodesktop}\Cut Assistant Next"; Filename: "{app}\CutAssistantNext.App.exe"; WorkingDir: "{app}"

