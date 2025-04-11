## ����һЩ Nuget ��
```
�� CommunityToolkit.MVVM
�� Microsoft.Extensions.DependencyInjection
�� MaterialDesignThemes
�� MaterialDesignThemes.MahApps
�� Masuit.Tools.Core https://www.masuit.tools/api.html
�� Microsoft.Extensions.Configuration
�� Microsoft.Extensions.Configuration.Json
�� Microsoft.Extensions.Configuration.Binder 
�� Microsoft.Extensions.Options 
�� SqlSugarCore
�� NLog.Extensions.Logging
```
## View ����
```xml
xmlns:vm="clr-namespace:TulingScadaSystem.ViewModels"
xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
xmlns:materialDesign="http://materialdesigninxaml.net/winfx/xaml/themes"
d:DataContext="{d:DesignInstance vm:MainViewModel}"
d:DesignHeight="800"
d:DesignWidth="1200"
Background="{DynamicResource MaterialDesign.Brush.Background}"
FontFamily="Microsoft YaHei"
FontSize="16"
TextElement.FontSize="16"
TextElement.FontWeight="Regular"
TextElement.Foreground="{DynamicResource MaterialDesignBody}"
```
## ͼ�������һ��
```xml
<StackPanel Margin="10" HorizontalAlignment="Center" Orientation="Horizontal">
                <materialDesign:PackIcon Width="50"
                                         Height="50"
                                         Margin="5" Foreground="#1b2755"
                                         VerticalAlignment="Center" Kind="ChartScatterPlotHexbin"/>
                <TextBlock HorizontalAlignment="Center" 
                           VerticalAlignment="Center"
                           FontSize="40"
                           Foreground="#1b2755"
                           Style="{StaticResource MaterialDesignTitleSmallTextBlock}"
                           Text="��Ϳ����SCADAϵͳ"/>
</StackPanel>   
```
## ListView ��  �¼�ת����
```xml
   <i:Interaction.Triggers>
                 <i:EventTrigger EventName="PreviewMouseUp">
                         <i:InvokeCommandAction Command="{Binding RelativeSource={RelativeSource Mode=FindAncestor, AncestorType=UserControl}, Path=DataContext.NavigationCommand}" CommandParameter="{Binding}" />
               </i:EventTrigger>
   </i:Interaction.Triggers>
```
## ��־����������
```C#
  //������־���ܺͲ�����ȡ
  private static readonly Logger _logger = LogManager.GetCurrentClassLogger(); // ֱ��ʹ�� 
  private readonly RootParam _rootParam;  //DI
```
## Bool ת string
```xml
 <UserControl.Resources>
        <conv:BoolToStringConverter x:Key="Bool2Bg"
                                    FalseValue="White"
                                    TrueValue="#E3F2FD" />
    </UserControl.Resources>
```

## MainView
``` xml


```


## NLog ʹ��ʾ��
``` C#
using System;
using NLog;

namespace GlucacxeScadaSystem.Services
{
    public static class LogDemo
    {
        // Ĭ�� logger������Ϊ������
        private static readonly Logger _defaultLogger = LogManager.GetCurrentClassLogger();

        // �Զ���ģ�� logger
        private static readonly Logger _plcLogger = LogManager.GetLogger("PLC");
        private static readonly Logger _dbLogger = LogManager.GetLogger("Database");

        public static void Run()
        {
            // ==== Ĭ����־��ʾ ====
            _defaultLogger.Trace("���� Trace ������־");
            _defaultLogger.Debug("���� Debug ������־");
            _defaultLogger.Info("��������");
            _defaultLogger.Warn("����һ��������Ϣ");
            _defaultLogger.Error("������һ�����󣬵�û���쳣");
            _defaultLogger.Fatal("���ش���");

            // ==== �쳣��־��ʾ ====
            try
            {
                int a = 10, b = 0;
                int c = a / b; // �׳��쳣
            }
            catch (Exception ex)
            {
                _defaultLogger.Error(ex, "ִ�г�������ʱ�����쳣");
            }

            // ==== ģ�������־ ====
            _plcLogger.Info("PLC ��ʼ�����");
            _plcLogger.Warn("PLC ͨѶ�ӳ�");

            _dbLogger.Info("���ݿ����ӳɹ�");
            _dbLogger.Error("���ݿ��ѯʧ��");
        }
    }
}

```
## NLog ����ʾ��
``` json
"NLog": {
  "internalLogLevel": "Info",
  "internalLogFile": "${currentdir}/Logs/ϵͳ������־.log",
  "extensions": [
    { "assembly": "NLog.Extensions.Logging" }
  ],
  "targets": {
    "default": {
      "type": "File",
      "fileName": "${currentdir}/Logs/${shortdate}/app.log",
      "layout": "${longdate}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}"
    },
    "plc": {
      "type": "File",
      "fileName": "${currentdir}/Logs/${shortdate}/plc.log",
      "layout": "${longdate}|${level}|${logger}|${message} ${exception:format=tostring}"
    },
    "db": {
      "type": "File",
      "fileName": "${currentdir}/Logs/${shortdate}/db.log",
      "layout": "${longdate}|${level}|${logger}|${message} ${exception:format=tostring}"
    }
  },
  "rules": [
    { "logger": "PLC", "minLevel": "Trace", "writeTo": "plc" },
    { "logger": "Database", "minLevel": "Trace", "writeTo": "db" },
    { "logger": "*", "minLevel": "Trace", "writeTo": "default" }
  ]
}

```

## ��������
``` C#
public class SomeService
{
    private readonly SqlParam _sqlParam;
    private readonly SystemParam _systemParam;

    public SomeService(SqlParam sqlParam, SystemParam systemParam)
    {
        _sqlParam = sqlParam;
        _systemParam = systemParam;
    }
}

```
