using System.Windows;
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

            containerRegistry.RegisterForNavigation<LoginView>();
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
