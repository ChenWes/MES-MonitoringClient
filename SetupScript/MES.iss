; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!


[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)

AppId={{F6C5E189-481C-4249-96B0-D30C7E6CB320}
;应用名称
AppName=MES Monitoring Client
;应用版本号
AppVersion=1.2.16
;AppVerName=MES Monitoring Client 1.0
;应用发布方
AppPublisher=广东翠峰机器人科技股份有限公司
;安装目录名称
DefaultDirName={pf64}\MES-Monitoring-Client
;安装目录不可选择
DisableDirPage=auto
;安装后不会出现在开始应用菜单
DisableProgramGroupPage=no
;安装包文件名
OutputBaseFilename=MES-MonitoringClient-Setup
;压缩包
Compression=lzma
SolidCompression=yes
;安装包图标文件
SetupIconFile=D:\document\mes\setup.ico
;安装时需要提供管理员权限
PrivilegesRequired=admin
;许可文件
LicenseFile=D:\report\wes\MES-MonitoringClient\SetupScript\License.txt


[Languages]Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "chs"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"

[Tasks]
;桌面增加快捷图标Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
;快捷方式Name: quicklaunchicon; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:CreateQuickLaunchIcon}"; Flags: unchecked


[Files]
;安装文件

;Client的文件夹
;Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\*"; DestDir: "{app}\Client"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\DnsClient.dll"; DestDir: "{app}\Client"; Flags: ignoreversion;  Components: Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\DnsClient.xml"; DestDir: "{app}\Client"; Flags: ignoreversion;  Components: Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\LiveCharts.dll"; DestDir: "{app}\Client"; Flags: ignoreversion;  Components: Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\LiveCharts.pdb"; DestDir: "{app}\Client"; Flags: ignoreversion;  Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\LiveCharts.WinForms.dll"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\LiveCharts.WinForms.pdb"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\LiveCharts.WinForms.xml"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\LiveCharts.Wpf.dll"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\LiveCharts.Wpf.pdb"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\LiveCharts.Wpf.xml"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client 
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\LiveCharts.xml"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\MES-MonitoringClient.exe"; DestDir: "{app}\Client"; Flags: ignoreversion;  Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\MES-MonitoringClient.exe.config"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\MES-MonitoringClient.pdb"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\MongoDB.Bson.dll"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\MongoDB.Bson.xml"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\MongoDB.Driver.Core.dll"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\MongoDB.Driver.Core.xml"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\MongoDB.Driver.dll"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\MongoDB.Driver.xml"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client;Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\RabbitMQ.Client.dll"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client;Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\RabbitMQ.Client.xml"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\System.Buffers.dll"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\System.Runtime.InteropServices.RuntimeInformation.dll"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\Newtonsoft.Json.dll"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\Newtonsoft.Json.pdb"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\Newtonsoft.Json.xml"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\log4net.dll"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\log4net.xml"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\QRCoder.dll"; DestDir: "{app}\Client"; Flags: ignoreversion; Components:Client
;如果有日志，则复制日志，如果没有，则不复制;Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringClient\bin\Debug\log.log"; DestDir: "{app}\Client"; Flags: ignoreversion skipifsourcedoesntexist; Components:Service 
;Service的文件夹
;Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\*"; DestDir: "{app}\Service"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\DnsClient.dll"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\DnsClient.xml"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\MES-MonitoringService.exe"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\MES-MonitoringService.exe.config"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\MES-MonitoringService.pdb"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\MongoDB.Bson.dll"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\MongoDB.Bson.xml"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\MongoDB.Driver.Core.dll"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\MongoDB.Driver.Core.xml"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\MongoDB.Driver.dll"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\MongoDB.Driver.xml"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\Newtonsoft.Json.dll"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\Newtonsoft.Json.pdb"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\Newtonsoft.Json.xml"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\RabbitMQ.Client.dll"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\RabbitMQ.Client.xml"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\System.Buffers.dll"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\System.Runtime.InteropServices.RuntimeInformation.dll"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\Topshelf.dll"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\Topshelf.xml"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\Newtonsoft.Json.dll"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\Newtonsoft.Json.pdb"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\Newtonsoft.Json.xml"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\log4net.dll"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\log4net.xml"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\Microsoft.Diagnostics.Tracing.EventSource.dll"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service
Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\Microsoft.Diagnostics.Tracing.EventSource.xml"; DestDir: "{app}\Service"; Flags: ignoreversion; Components:Service

;DefendService的文件夹
;Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\*"; DestDir: "{app}\Service"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\log4net.dll"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\log4net.xml"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\MES-Service-Defend.exe"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\MES-Service-Defend.exe.config"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\MES-Service-Defend.pdb"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\MongoDB.Bson.dll"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\MongoDB.Bson.xml"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\MongoDB.Driver.Core.dll"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\MongoDB.Driver.Core.xml"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\MongoDB.Driver.dll"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\MongoDB.Driver.xml"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\Newtonsoft.Json.dll"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\Newtonsoft.Json.pdb"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\Newtonsoft.Json.xml"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\RabbitMQ.Client.dll"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\RabbitMQ.Client.xml"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\System.Runtime.InteropServices.RuntimeInformation.dll"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\Topshelf.dll"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService
Source: "D:\report\wes\MES-MonitoringClient\MES-Service-Defend\bin\Debug\Topshelf.xml"; DestDir: "{app}\DefendService"; Flags: ignoreversion; Components:DefendService

Source: "D:\report\wes\MES-MonitoringClient\Mes_Update\bin\Debug\ICSharpCode.SharpZipLib.dll"; DestDir: "{app}\Client_Update"; Flags: ignoreversion;  Components: Client_Update
Source: "D:\report\wes\MES-MonitoringClient\Mes_Update\bin\Debug\Mes_Update.exe"; DestDir: "{app}\Client_Update"; Flags: ignoreversion;  Components: Client_Update
Source: "D:\report\wes\MES-MonitoringClient\Mes_Update\bin\Debug\Mes_Update.exe.config"; DestDir: "{app}\Client_Update"; Flags: ignoreversion;  Components: Client_Update
Source: "D:\report\wes\MES-MonitoringClient\Mes_Update\bin\Debug\Mes_Update.pdb"; DestDir: "{app}\Client_Update"; Flags: ignoreversion;  Components: Client_Update
;如果有日志，则复制日志，如果没有，则不复制
;Source: "D:\report\wes\MES-MonitoringClient\MES-MonitoringService\bin\Debug\log.log"; DestDir: "{app}\Service"; Flags: ignoreversion skipifsourcedoesntexist; Components:Service 

;NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
;桌面图标
Name: "{commondesktop}\MES Monitoring Client"; Filename: "{app}\Client\MES-MonitoringClient.exe"; Tasks: desktopicon
;在“开始”--“程序”里，添加一个开始的快捷方式
Name: "{commonprograms}\MES Client\MES Monitoring Client"; Filename: "{app}\Client\MES-MonitoringClient.exe"
;在“开始”--“程序”里，添加一个卸载的快捷方式
Name: "{commonprograms}\MES Client\Uninstall Application"; Filename: "{uninstallexe}"

[Registry]
;开机启动
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "MES Monitoring Client"; ValueData: """{app}\Client\MES-MonitoringClient.exe"""; Flags: uninsdeletevalue


[run]
;两种方法都可以安装服务，上面的可以将服务安装好，但不能直接运行
;Filename: {sys}\sc.exe; Parameters: "create MESUploadDataService start= auto binPath= ""{app}\Service\MES-MonitoringService.exe""" ; Flags: runhidden
;以下的方式可以直接运行，其中有Components:Service;当选中了服务才会安装服务      
;Flags:postinstall点击完成后，才会进行服务的安装，因为在处理RabbitMQ的服务器参数时，不会直接替换参数的
Filename: "{app}\Service\MES-MonitoringService.exe"; Description:"安装并运行MES客户端数据上传服务"; Parameters: " install start"; Components:Service; Flags:postinstall runhidden hidewizard;
;安装Defend服务
Filename: "{app}\DefendService\MES-Service-Defend.exe"; Description:"安装并运行MES服务端守护服务"; Parameters: " install start"; Components:DefendService; Flags:postinstall runhidden hidewizard;
;安装完成后启动应用
Filename: "{app}\Client\MES-MonitoringClient.exe"; Description: "{cm:LaunchProgram,MES Monitoring Client}"; Flags:postinstall skipifsilent unchecked      


[UninstallRun]
;卸载守护服务
Filename: {sys}\sc.exe; Parameters: "stop MESServiceDefend" ; Flags: runhidden; Components:DefendService
Filename: {sys}\sc.exe; Parameters: "delete MESServiceDefend" ; Flags: runhidden; Components:DefendService
;卸载时，停止服务并删除服务
Filename: {sys}\sc.exe; Parameters: "stop MESUploadDataService" ; Flags: runhidden; Components:Service
Filename: {sys}\sc.exe; Parameters: "delete MESUploadDataService" ; Flags: runhidden; Components:Service

[Messages]
;安装时，windows任务栏提示标题
SetupAppTitle=MES Monitoring Client Setup
;安装时，安装引导标题
SetupWindowTitle=MES Monitoring Client Setup
;在界面左下角加文字
BeveledLabel=广东翠峰机器人股份有限公司
;卸载对话框说明
ConfirmUninstall=您真的想要从电脑中卸载 %1 吗?%n%n按 [是] 则完全删除 %1 以及它的所有组件;%n按 [否]则让软件继续留在您的电脑上.


[Types]
Name: "normaltype"; Description: "Normal Setup"
Name: "custom";     Description: "Custom Installation"; Flags: iscustom

[Components]
Name: "Client";     Description: "应用界面";  Types: normaltype custom
Name: "Service";    Description: "后台服务";  Types: normaltype custom
Name: "DefendService";    Description: "守护服务";  Types: normaltype custom
Name: "Client_Update";    Description: "更修程序";  Types: normaltype custom

[Code]
var CustomPage: TInputQueryWizardPage;
var CustomAPIPage: TInputQueryWizardPage;

//设置Rabbit Server Host
function NextButtonClick(CurPage: Integer): Boolean;
var
  str: string;
  strFilename: string;
  HostEmptyChecked: Boolean;

begin
  Result := true;  

  if CurPage = wpSelectComponents then
   begin      
      if (CustomPage = nil) then
      begin
        // Set Custom Page initial values
        CustomPage := CreateInputQueryPage(wpSelectComponents, 
        'RabbitMQ 配置', 'MES服务信息配置', 
        '请输入RabbitMQ Server Host地址，然后点击 下一步 按钮');
        CustomPage.Add('RabbitMQ Server Host:', False);        
        CustomPage.Values[0] := '172.19.0.153';                
      end;
   end;

   begin      
      if (CustomAPIPage = nil) then
      begin
        // Set Custom Page initial values
        CustomAPIPage := CreateInputQueryPage(wpSelectComponents, 
        'API 配置', 'MES服务信息配置', 
        '请输入API Server Host地址，然后点击 下一步 按钮');
        CustomAPIPage.Add('API Server Host:', False);        
        CustomAPIPage.Values[0] := '172.19.0.153';                
      end;
   end;

  if CurPage = wpFinished then
   begin
      //找到文件地下
      strFilename := ExpandConstant('{app}\Service\MES-MonitoringService.exe.config');

      if FileExists(strFilename) then
      begin
        // Replace the values in the .config file and save it
        LoadStringFromFile(strFilename, str);
        //通过替换完整的key，找到所有<add key="RabbitMQServerHostName" value="localhost"/>内容并替换成以下值
        StringChangeEx(str, '<add key="RabbitMQServerHostName" value="localhost" />','<add key="RabbitMQServerHostName" value="'+CustomPage.Values[0]+'" />', True); 
        StringChangeEx(str, '<add key="BackendServerHost" value="localhost" />','<add key="BackendServerHost" value="'+CustomAPIPage.Values[0]+'" />', True); 
        //application need check mongodb service replace to 1  
        StringChangeEx(str, '<add key="CheckMongoDBService" value="0"/>','<add key="CheckMongoDBService" value="1"/>', True);
        SaveStringToFile(strFilename, str, False);             
      end;
  

      //找到文件地下
      strFilename := ExpandConstant('{app}\Client\MES-MonitoringClient.exe.config');

      if FileExists(strFilename) then
      begin
        // Replace the values in the .config file and save it
        LoadStringFromFile(strFilename, str);
        //通过替换完整的key，找到所有<add key="RabbitMQServerHostName" value="localhost"/>内容并替换成以下值
        StringChangeEx(str, '<add key="BackendServerHost" value="localhost" />','<add key="BackendServerHost" value="'+CustomAPIPage.Values[0]+'" />', True);
        //application need check mongodb service replace to 1  
        StringChangeEx(str, '<add key="CheckMongoDBService" value="0"/>','<add key="CheckMongoDBService" value="1"/>', True);        
        SaveStringToFile(strFilename, str, False);    
        
         //守护服务
      strFilename := ExpandConstant('{app}\DefendService\MES-Service-Defend.exe.config');

      if FileExists(strFilename) then
      begin
        // Replace the values in the .config file and save it
        LoadStringFromFile(strFilename, str);
        //通过替换完整的key，找到所有<add key="RabbitMQServerHostName" value="localhost"/>内容并替换成以下值
        StringChangeEx(str, '<add key="RabbitMQServerHostName" value="localhost" />','<add key="RabbitMQServerHostName" value="'+CustomPage.Values[0]+'" />', True); 
        //application need check mongodb service replace to 1  
        //StringChangeEx(str, '<add key="CheckMongoDBService" value="0"/>','<add key="CheckMongoDBService" value="1"/>', True);        
        SaveStringToFile(strFilename, str, False);             
      end;         
      end;
   end;
end;

[Code]
//设置界面文字颜色
procedure InitializeWizard(); 
begin 
  //WizardForm.WELCOMELABEL1.Font.Color:= clGreen;//设置开始安装页面第一段文字的颜色为绿色 
  //WizardForm.WELCOMELABEL2.Font.Color:= clOlive;//设置开始安装页面第二段文字的颜色为橄榄绿 
  //WizardForm.PAGENAMELABEL.Font.Color:= clRed;//设置许可协议页面第一段文字的颜色为红色 
  //WizardForm.PAGEDESCRIPTIONLABEL.Font.Color:= clBlue; //设置许可协议页面第二段文字的颜色为蓝色 
  WizardForm.MainPanel.Color:= clWhite;//设置窗格的颜色为白色
 
end;

//卸载后打开网址
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ErrorCode: Integer;
begin
  case CurUninstallStep of
    usUninstall:
      begin        
        // 正在卸载
      end;
    usPostUninstall:
      begin
        //卸载完成       
        //ShellExec('open', 'http://www.cfmm.com.cn/', '', '', SW_SHOW, ewNoWait, ErrorCode)
      end;
  end;
end;

