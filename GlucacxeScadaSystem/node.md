## 引入一些 Nuget 包
```
● CommunityToolkit.MVVM
● Microsoft.Extensions.DependencyInjection
● MaterialDesignThemes
● MaterialDesignThemes.MahApps
● Masuit.Tools.Core https://www.masuit.tools/api.html
● Microsoft.Extensions.Configuration
● Microsoft.Extensions.Configuration.Json
● Microsoft.Extensions.Configuration.Binder 
● Microsoft.Extensions.Options 
● SqlSugarCore
● NLog.Extensions.Logging
```
## View 必有
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
## 图标和文字一起
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
                           Text="喷涂工艺SCADA系统"/>
</StackPanel>   
```
## ListView 绑定  事件转命令
```xml
   <i:Interaction.Triggers>
                 <i:EventTrigger EventName="PreviewMouseUp">
                         <i:InvokeCommandAction Command="{Binding RelativeSource={RelativeSource Mode=FindAncestor, AncestorType=UserControl}, Path=DataContext.NavigationCommand}" CommandParameter="{Binding}" />
               </i:EventTrigger>
   </i:Interaction.Triggers>
```
## 日志及参数配置
```C#
  //测试日志功能和参数获取
  private static readonly Logger _logger = LogManager.GetCurrentClassLogger(); // 直接使用 
  private readonly RootParam _rootParam;  //DI
```
## Bool 转 string
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


## NLog 使用示例
``` C#
using System;
using NLog;

namespace GlucacxeScadaSystem.Services
{
    public static class LogDemo
    {
        // 默认 logger（名称为类名）
        private static readonly Logger _defaultLogger = LogManager.GetCurrentClassLogger();

        // 自定义模块 logger
        private static readonly Logger _plcLogger = LogManager.GetLogger("PLC");
        private static readonly Logger _dbLogger = LogManager.GetLogger("Database");

        public static void Run()
        {
            // ==== 默认日志演示 ====
            _defaultLogger.Trace("这是 Trace 级别日志");
            _defaultLogger.Debug("这是 Debug 级别日志");
            _defaultLogger.Info("程序启动");
            _defaultLogger.Warn("这是一个警告信息");
            _defaultLogger.Error("发生了一个错误，但没有异常");
            _defaultLogger.Fatal("严重错误！");

            // ==== 异常日志演示 ====
            try
            {
                int a = 10, b = 0;
                int c = a / b; // 抛出异常
            }
            catch (Exception ex)
            {
                _defaultLogger.Error(ex, "执行除法操作时发生异常");
            }

            // ==== 模块分类日志 ====
            _plcLogger.Info("PLC 初始化完成");
            _plcLogger.Warn("PLC 通讯延迟");

            _dbLogger.Info("数据库连接成功");
            _dbLogger.Error("数据库查询失败");
        }
    }
}

```
## NLog 配置示例
``` json
"NLog": {
  "internalLogLevel": "Info",
  "internalLogFile": "${currentdir}/Logs/系统报错日志.log",
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

## 参数配置
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
