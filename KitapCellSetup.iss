[Setup]
; Genel Kurulum Ayarları
AppName=KitapCell Kutuphane Otomasyonu
AppVersion=1.0.3
AppPublisher=yagizerhan
DefaultDirName={autopf}\KitapCell
DefaultGroupName=KitapCell
OutputDir=.\
OutputBaseFilename=KitapCell_Setup_v1.0.3
Compression=lzma2
SolidCompression=yes
; SetupIconFile iptal edildi çünkü app.ico dosyası Inno Setup için çok büyük. Varsayılan ikon kullanılacak.
UninstallDisplayIcon={app}\KitapCell.exe
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin

[Dirs]
; Program Files klasöründe yazma izni (Assets ve Uploads için gerekli)
Name: "{app}"; Permissions: users-modify

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Publish edilen tüm dosyaları ve alt klasörleri kurulum dizinine kopyalar
Source: "KitapCell\bin\Publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
; Başlat Menüsü Kısayolu
Name: "{group}\KitapCell"; Filename: "{app}\KitapCell.exe"; IconFilename: "{app}\Resources\app.ico"
; Masaüstü Kısayolu
Name: "{autodesktop}\KitapCell"; Filename: "{app}\KitapCell.exe"; IconFilename: "{app}\Resources\app.ico"; Tasks: desktopicon

[Run]
; Kurulum bittikten sonra uygulamayı başlatma seçeneği
Filename: "{app}\KitapCell.exe"; Description: "{cm:LaunchProgram,KitapCell}"; Flags: nowait postinstall skipifsilent
