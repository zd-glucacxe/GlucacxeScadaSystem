using System.Windows;
using GlucacxeScadaSystem.ViewModels;
using GlucacxeScadaSystem.Views;
using Prism.Ioc;

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
        }
    }
}
