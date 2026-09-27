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
OutputBaseFilename=CutAssistantNext-0.2.0-Build6-setup-TEST2

Compression=lzma2
SolidCompression=yes

UninstallDisplayIcon={app}\CutAssistantNext.App.exe

[Languages]
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Files]
Source: "..\artifacts\installer-input\gpac\gpac-26.07-rev0-ga07cbfff-master-x64.exe"; Flags: dontcopy noencryption
Source: "..\artifacts\installer-stage\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Cut Assistant Next"; Filename: "{app}\CutAssistantNext.App.exe"; WorkingDir: "{app}"
Name: "{autodesktop}\Cut Assistant Next"; Filename: "{app}\CutAssistantNext.App.exe"; WorkingDir: "{app}"

[Code]
function GpacExistsInRegistry(const Root: Integer): Boolean;
var
  InstallDir: String;
  UninstallCommand: String;
  Candidate: String;
  ClosingQuote: Integer;
  ExePosition: Integer;
begin
  Result := False;

  if RegQueryStringValue(
       Root,
       'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\GPAC',
       'InstallLocation',
       InstallDir) then
  begin
    InstallDir := RemoveQuotes(Trim(InstallDir));

    if InstallDir <> '' then
    begin
      Candidate := AddBackslash(InstallDir) + 'MP4Box.exe';

      if FileExists(Candidate) then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;

  if not RegQueryStringValue(
       Root,
       'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\GPAC',
       'UninstallString',
       UninstallCommand) then
    Exit;

  UninstallCommand := Trim(UninstallCommand);

  if UninstallCommand = '' then
    Exit;

  if UninstallCommand[1] = '"' then
  begin
    Delete(UninstallCommand, 1, 1);
    ClosingQuote := Pos('"', UninstallCommand);

    if ClosingQuote = 0 then
      Exit;

    UninstallCommand :=
      Copy(UninstallCommand, 1, ClosingQuote - 1);
  end
  else
  begin
    ExePosition := Pos('.exe', Lowercase(UninstallCommand));

    if ExePosition = 0 then
      Exit;

    UninstallCommand :=
      Copy(UninstallCommand, 1, ExePosition + 3);
  end;

  if CompareText(
       ExtractFileName(UninstallCommand),
       'uninstall.exe') <> 0 then
    Exit;

  Candidate :=
    AddBackslash(ExtractFileDir(UninstallCommand)) +
    'MP4Box.exe';

  Result := FileExists(Candidate);
end;

function IsGpacInstalled(): Boolean;
begin
  Result :=
    GpacExistsInRegistry(HKLM32) or
    GpacExistsInRegistry(HKLM64) or
    GpacExistsInRegistry(HKCU32) or
    GpacExistsInRegistry(HKCU64);
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  InstallerPath: String;
  ResultCode: Integer;
begin
  if CurStep <> ssPostInstall then
    Exit;

  if IsGpacInstalled() then
  begin
    Log('GPAC ist bereits installiert. Installation wird uebersprungen.');
    Exit;
  end;

  if MsgBox(
       'GPAC mit MP4Box wurde nicht gefunden.' + #13#10 + #13#10 +
       'Soll der mitgelieferte offizielle GPAC-Installer ' +
       'jetzt gestartet werden?' + #13#10 + #13#10 +
       'Ohne MP4Box kann CAN Cutlists erstellen, ' +
       'aber keine Videos schneiden.',
       mbConfirmation,
       MB_YESNO or MB_DEFBUTTON2) <> IDYES then
  begin
    MsgBox(
      'GPAC wurde nicht installiert. Du kannst den ' +
      'MP4Box-Pfad spaeter in CAN konfigurieren.',
      mbInformation,
      MB_OK);
    Exit;
  end;

  ExtractTemporaryFile(
    'gpac-26.07-rev0-ga07cbfff-master-x64.exe');

  InstallerPath := ExpandConstant(
    '{tmp}\gpac-26.07-rev0-ga07cbfff-master-x64.exe');

  if not Exec(
       InstallerPath,
       '',
       '',
       SW_SHOWNORMAL,
       ewWaitUntilTerminated,
       ResultCode) then
  begin
    MsgBox(
      'Der GPAC-Installer konnte nicht gestartet werden: ' +
      SysErrorMessage(ResultCode),
      mbError,
      MB_OK);
    Exit;
  end;

  if (ResultCode <> 0) or (not IsGpacInstalled()) then
  begin
    MsgBox(
      'Die GPAC-Installation konnte nicht bestaetigt werden.' + #13#10 +
      'Bitte GPAC pruefen oder MP4Box spaeter manuell konfigurieren.',
      mbError,
      MB_OK);
    Exit;
  end;

  MsgBox(
    'GPAC wurde erfolgreich erkannt. CAN kann MP4Box ' +
    'jetzt automatisch verwenden.',
    mbInformation,
    MB_OK);
end;
