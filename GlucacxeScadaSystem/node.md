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
## ListView ��
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
    private readonly ILogger<MainViewModel> _logger;
    private RootParam RootParamProp;
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