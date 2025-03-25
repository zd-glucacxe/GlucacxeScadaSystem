using System.Windows;
using System.Windows.Controls;
using GlucacxeScadaSystem.Services;
using GlucacxeScadaSystem.ViewModels;
using GlucacxeScadaSystem.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace GlucacxeScadaSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            //return Container.Resolve<MainWindow>();
            return Container.Resolve<ShellView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册单例
            containerRegistry.RegisterSingleton<ShellViewModel>();
            containerRegistry.RegisterSingleton<LoginViewModel>();
            containerRegistry.RegisterSingleton<IndexViewModel>();
            containerRegistry.RegisterSingleton<ChartViewModel>();
            containerRegistry.RegisterSingleton<UserViewModel>();
            containerRegistry.RegisterSingleton<ReportViewModel>();
            containerRegistry.RegisterSingleton<ParamsViewModel>();
            containerRegistry.RegisterSingleton<LogViewModel>();
            containerRegistry.RegisterSingleton<FormulaViewModel>();
            containerRegistry.RegisterSingleton<DataQueryViewModel>();
            containerRegistry.RegisterSingleton<MainViewModel>();
            containerRegistry.RegisterSingleton<DeviceViewModel>();


            // UserSession 注册为单例
            containerRegistry.RegisterSingleton<UserSession>();


            //导航
            containerRegistry.RegisterForNavigation<LoginView>();
            containerRegistry.RegisterForNavigation<MainView>();

            containerRegistry.RegisterForNavigation<IndexView>();
            containerRegistry.RegisterForNavigation<DeviceView>();
            containerRegistry.RegisterForNavigation<UserView>();
        }

        /// <summary>
        /// 确保 MainRegion 先初始化，然后再执行 RequestNavigate()
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate("MainRegion", nameof(LoginView));
        }


    }
}
