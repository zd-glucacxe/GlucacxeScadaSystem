using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using GlucacxeScadaSystem.Models;
using GlucacxeScadaSystem.Services;
using GlucacxeScadaSystem.ViewModels;
using GlucacxeScadaSystem.Views;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
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

            // 注册参数绑定对象
            ConfigureJsonByBinder(containerRegistry);

            // 注册单例
            containerRegistry.RegisterSingleton<ShellViewModel>();
            containerRegistry.RegisterSingleton<LoginViewModel>();
            containerRegistry.RegisterSingleton<IndexViewModel>();
            containerRegistry.RegisterSingleton<ChartViewModel>();
            containerRegistry.RegisterSingleton<UserViewModel>();
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


        private void ConfigureJsonByBinder(IContainerRegistry containerRegistry)
        {
            var cfgBuilder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Path.Combine(Environment.CurrentDirectory, "Configs"))
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);

            var configuration = cfgBuilder.Build();

            // 注册配置对象
            var rootParam = new RootParam();
            configuration.Bind(rootParam);
            containerRegistry.RegisterInstance(rootParam);

            // 注册 IConfiguration
            containerRegistry.RegisterInstance<IConfiguration>(configuration);


            // 参数配置及映射
            containerRegistry.RegisterInstance(rootParam.SqlParam);
            containerRegistry.RegisterInstance(rootParam.SystemParam);
            containerRegistry.RegisterInstance(rootParam.PlcParam);


            // 初始化日志
            LogService.AddLog(configuration);

            // 改造 SqlSugarHelper
            var dbTypeRes = Enum.TryParse<SqlSugar.DbType>(configuration["SqlParam:DbType"], out var dbType);
            var connectionStringRes = configuration["SqlParam:ConnectionString"];

            if (dbTypeRes)
            {
                SqlSugarHelper.AddSqlSugarSetup(dbType, connectionStringRes);
            }


           
           
        }


    }
}
