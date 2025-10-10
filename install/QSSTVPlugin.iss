#define MyAppName "QSSTV Plugin"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Acuantico Power"
#define MyAppURL "https://ve3nea.github.io/SkyRoof"
#define MyAppPluginDll "skyroof_sstv.dll"

[Setup]
AppId={{d2df1a7a-9dc7-42c7-ac64-0f9e7039370b}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
DefaultDirName={autopf}\Afreet\SkyRoof
DisableProgramGroupPage=yes
DisableDirPage=no
UsePreviousAppDir=yes
AllowNoIcons=yes
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible
OutputDir=.
OutputBaseFilename=SkyRoof_QSSTV_Plugin_Setup
SetupIconFile=..\SkyRoof\SkyRoof.ico
Compression=lzma
SolidCompression=true
DisableReadyPage=true
ShowLanguageDialog=yes
InfoBeforeFile=QSSTVPlugin_credit.txt

[Languages]
Name: spanish; MessagesFile: compiler:Languages\Spanish.isl
Name: english; MessagesFile: compiler:Default.isl

[Files]
Source: ..\SkyRoof\bin\x64\Release\net9.0-windows7.0\SkyRoof.dll; DestDir: {app}; Flags: ignoreversion overwritereadonly
Source: ..\SkyRoof\bin\x64\Release\net9.0-windows7.0\SkyRoof.exe; DestDir: {app}; Flags: ignoreversion overwritereadonly
Source: ..\SkyRoof\bin\x64\Release\net9.0-windows7.0\SkyRoof.deps.json; DestDir: {app}; Flags: ignoreversion overwritereadonly
Source: ..\SkyRoof\bin\x64\Release\net9.0-windows7.0\SkyRoof.runtimeconfig.json; DestDir: {app}; Flags: ignoreversion overwritereadonly
Source: ..\SkyRoof\native\{#MyAppPluginDll}; DestDir: {app}; Flags: ignoreversion overwritereadonly
Source: ..\SkyRoof\native\{#MyAppPluginDll}; DestDir: {app}\native; Flags: ignoreversion overwritereadonly

[Dirs]
Name: {app}\native

[UninstallDelete]
Type: files; Name: "{app}\VERSION_OVERRIDE"

[Code]
const
  LOAD_LIBRARY_AS_DATAFILE = $00000002;
  RT_VERSION = 16;
  VS_VERSION_RESOURCE_ID = 1;
  LANG_FALLBACK = $0409;

var
  PreparationDone: Boolean;
  StoredVersion: string;
  ExeVersionBacked: Boolean;
  DllVersionBacked: Boolean;
  ExeVersionBackupPath: string;
  DllVersionBackupPath: string;

function LoadLibraryEx(lpFileName: string; hFile: Integer; dwFlags: Cardinal): THandle;
  external 'LoadLibraryExW@kernel32.dll stdcall';
function FreeLibrary(hModule: THandle): Boolean;
  external 'FreeLibrary@kernel32.dll stdcall';
function FindResourceEx(hModule: THandle; lpType, lpName: LongWord; wLanguage: Word): THandle;
  external 'FindResourceExW@kernel32.dll stdcall';
function SizeofResource(hModule: THandle; hResInfo: THandle): Cardinal;
  external 'SizeofResource@kernel32.dll stdcall';
function LoadResource(hModule: THandle; hResInfo: THandle): THandle;
  external 'LoadResource@kernel32.dll stdcall';
function LockResource(hResData: THandle): LongWord;
  external 'LockResource@kernel32.dll stdcall';
function BeginUpdateResource(FileName: string; DeleteExisting: Boolean): THandle;
  external 'BeginUpdateResourceW@kernel32.dll stdcall';
function UpdateResource(hUpdate: THandle; lpType, lpName: LongWord; wLanguage: Word; lpData: LongWord; cbData: Cardinal): Boolean;
  external 'UpdateResourceW@kernel32.dll stdcall';
function EndUpdateResource(hUpdate: THandle; Discard: Boolean): Boolean;
  external 'EndUpdateResourceW@kernel32.dll stdcall';

procedure BackupVersionFile(const SourcePath, BackupName: string; out BackupPath: string; out Backed: Boolean);
begin
  BackupPath := '';
  Backed := False;
  if not FileExists(SourcePath) then
    Exit;

  BackupPath := ExpandConstant('{tmp}\') + BackupName;
  if FileExists(BackupPath) then
    DeleteFile(BackupPath);

  if CopyFile(SourcePath, BackupPath, False) then
    Backed := True
  else
    BackupPath := '';
end;

procedure RestoreVersionInfo(const SourcePath, TargetPath: string);
var
  module, resInfo, resData: THandle;
  size: Cardinal;
  dataPtr: LongWord;
  lang: Word;
  update: THandle;
begin
  if (SourcePath = '') or (TargetPath = '') then
    Exit;
  if not FileExists(SourcePath) then
    Exit;
  if not FileExists(TargetPath) then
    Exit;

  module := LoadLibraryEx(SourcePath, 0, LOAD_LIBRARY_AS_DATAFILE);
  if module = 0 then
    Exit;

  try
    lang := 0;

    resInfo := FindResourceEx(module, RT_VERSION, VS_VERSION_RESOURCE_ID, lang);
    if resInfo = 0 then
    begin
      lang := LANG_FALLBACK;
      resInfo := FindResourceEx(module, RT_VERSION, VS_VERSION_RESOURCE_ID, lang);
      if resInfo = 0 then
        Exit;
    end;

    size := SizeofResource(module, resInfo);
    if size = 0 then
      Exit;

    resData := LoadResource(module, resInfo);
    if resData = 0 then
      Exit;

    dataPtr := LockResource(resData);
    if dataPtr = 0 then
      Exit;

    update := BeginUpdateResource(TargetPath, False);
    if update = 0 then
      Exit;

    if not UpdateResource(update, RT_VERSION, VS_VERSION_RESOURCE_ID, lang, dataPtr, size) then
    begin
      EndUpdateResource(update, True);
      Exit;
    end;

    if not EndUpdateResource(update, False) then
      EndUpdateResource(update, True);
  finally
    FreeLibrary(module);
  end;
end;

function PosEx(const SubStr, S: string; Offset: Integer): Integer;
var
  temp: string;
begin
  if Offset <= 0 then
    Offset := 1;
  temp := Copy(S, Offset, MaxInt);
  Result := Pos(SubStr, temp);
  if Result > 0 then
    Result := Result + Offset - 1;
end;


function ExtractMajorMinor(const VersionStr: string): string;
var
  firstDot, secondDot: Integer;
begin
  firstDot := Pos('.', VersionStr);
  if firstDot = 0 then
  begin
    Result := VersionStr;
    Exit;
  end;

  secondDot := PosEx('.', VersionStr, firstDot + 1);
  if secondDot = 0 then
    Result := VersionStr
  else
    Result := Copy(VersionStr, 1, secondDot - 1);
end;


procedure WriteVersionOverride(const AppDir: string);
var
  exePath, versionStr, overrideValue: string;
begin
  exePath := AddBackslash(AppDir) + 'SkyRoof.exe';
  if GetVersionNumbersString(exePath, versionStr) then
  begin
    overrideValue := ExtractMajorMinor(versionStr);
    StoredVersion := overrideValue;
    if not SaveStringToFile(AddBackslash(AppDir) + 'VERSION_OVERRIDE', overrideValue, False) then
      MsgBox('No se pudo guardar el archivo VERSION_OVERRIDE.', mbError, MB_OK);
  end
  else
    StoredVersion := '';
end;

procedure PrepareTarget(const AppDir: string);
var
  exePath, dllPath: string;
begin
  if PreparationDone then
    Exit;

  exePath := AddBackslash(AppDir) + 'SkyRoof.exe';
  if not FileExists(exePath) then
    Exit;

  dllPath := AddBackslash(AppDir) + 'SkyRoof.dll';

  BackupVersionFile(exePath, 'SkyRoofExe.versionbak', ExeVersionBackupPath, ExeVersionBacked);
  BackupVersionFile(dllPath, 'SkyRoofDll.versionbak', DllVersionBackupPath, DllVersionBacked);
  WriteVersionOverride(AppDir);
  PreparationDone := True;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  if CurPageID = wpSelectDir then
  begin
    if not FileExists(ExpandConstant('{app}\SkyRoof.exe')) then
    begin
      MsgBox('No se encontro SkyRoof.exe en la carpeta seleccionada. Selecciona la instalacion existente de SkyRoof (x64).', mbError, MB_OK);
      Result := False;
    end;
    if Result then
      PrepareTarget(ExpandConstant('{app}'));
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssInstall then
    PrepareTarget(ExpandConstant('{app}'));

  if CurStep = ssPostInstall then
  begin
    if ExeVersionBacked then
      RestoreVersionInfo(ExeVersionBackupPath, ExpandConstant('{app}\SkyRoof.exe'));
    if DllVersionBacked then
      RestoreVersionInfo(DllVersionBackupPath, ExpandConstant('{app}\SkyRoof.dll'));
  end;
end;
