using GlucacxeScadaSystem.EventAggregator;
using GlucacxeScadaSystem.Helpers;
using GlucacxeScadaSystem.Models;
using GlucacxeScadaSystem.Views;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using NLog;
using HslCommunication.Profinet.Siemens;

namespace GlucacxeScadaSystem.ViewModels;

public class ShellViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private readonly IEventAggregator _eventAggregator;
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private SiemensS7Server _simulatedServer;

    public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
    {
        InitData();
        _regionManager = regionManager;
        NavigateToLoginView();
        _eventAggregator = eventAggregator;

        _eventAggregator.GetEvent<ChangUserEvent>().Subscribe(RedirectToLoginView);

        InitSiemensPlcServer();
    }

    private void InitSiemensPlcServer()
    {
        HslCommunication.Authorization.SetAuthorizationCode("d1c914f5-2159-49f0-9c7e-2a6c5cd29e55");
        _logger.Info($"开始仿真西门子Plc-1200 at {DateTime.Now}");
        //打开西门子模拟串口
        _simulatedServer = new HslCommunication.Profinet.Siemens.SiemensS7Server();
        _simulatedServer.ServerStart(102);
    }



    private void RedirectToLoginView()
    {
        if (_regionManager.Regions.ContainsRegionWithName("MainRegion"))
        {
            _regionManager.RequestNavigate("MainRegion", nameof(LoginView));
        }
    }

    /// <summary>
    ///  数据库初始化 ,创建表及一些数据
    /// </summary>
    private void InitData()
    {

        if (false)
        {
            SqlSugarHelper.Db.CodeFirst.InitTables(typeof(Menu));

            var menuList = new List<Menu>();
            menuList.Add(new Menu()
            {
                MenuName = "首页",
                Icon = "Home",
                View = "IndexView",
                Sort = 1,
            });
            menuList.Add(new Menu()
            {
                MenuName = "设备总控",
                Icon = "Devices",
                View = "DeviceView",
                Sort = 2,
            });
            menuList.Add(new Menu()
            {
                MenuName = "配方管理",
                Icon = "AirFilter",
                View = "FormulaView",
                Sort = 3,
            });
            menuList.Add(new Menu()
            {
                MenuName = "参数管理",
                Icon = "AlphabetCBoxOutline",
                View = "ParamsView",
                Sort = 4,
            });
            menuList.Add(new Menu()
            {
                MenuName = "数据查询",
                Icon = "DataUsage",
                View = "DataQueryView",
                Sort = 5,
            });
            menuList.Add(new Menu()
            {
                MenuName = "数据趋势",
                Icon = "ChartFinance",
                View = "ChartView",
                Sort = 6,
            });
            menuList.Add(new Menu()
            {
                MenuName = "报表管理",
                Icon = "FileReport",
                View = "ReportView",
                Sort = 7,
            });
            menuList.Add(new Menu()
            {
                MenuName = "日志管理",
                Icon = "NotebookOutline",
                View = "LogView",
                Sort = 8,
            });
            menuList.Add(new Menu()
            {
                MenuName = "用户管理",
                Icon = "UserMultipleOutline",
                View = "UserView",
                Sort = 9,
            });

            SqlSugarHelper.Db.Insertable(menuList).ExecuteCommand();
        }
    

        if (true)
        {
            // 第一次建表 则为true 随后为false
            if (true)
            {
                // 建库
                //SqlSugarHelper.Db.DbMaintenance.CreateDatabase();
                // 建表
                //SqlSugarHelper.Db.CodeFirst.InitTables(typeof(User));
                SqlSugarHelper.Db.CodeFirst.InitTables<ScadaReadData>();

            }
            // 插入数据 root user user1
            if (SqlSugarHelper.Db.Queryable<User>().Count() == 0)
            {
                var userList = new List<User>();
                userList.Add(new User() { UserName = "root", PassWord = "root", Role = 0 });
                userList.Add(new User() { UserName = "user", PassWord = "user", Role = 1 });
                userList.Add(new User() { UserName = "user1", PassWord = "user1", Role = 1});

                // 执行插入
                SqlSugarHelper.Db.Insertable(userList).ExecuteCommand();
            }

            // 插入数据  ScadaReadData
            if (SqlSugarHelper.Db.Queryable<ScadaReadData>().Count() == 0)
            {
                var scadaReadDataList = new List<ScadaReadData>();

                for (int i = 0; i < 100; i++)
                {
                    var scadaReadData = new ScadaReadData()
                    {
                        DegreasingSprayPumpPressure = GetRandomFloat(0.5f, 5.0f),
                        DegreasingPhValue = GetRandomFloat(6.0f, 9.0f),
                        RoughWashSprayPumpPressure = GetRandomFloat(1.0f, 4.0f),
                        PhosphatingSprayPumpPressure = GetRandomFloat(0.8f, 3.5f),
                        PhosphatingPhValue = GetRandomFloat(4.0f, 7.0f),
                        FineWashSprayPumpPressure = GetRandomFloat(1.2f, 4.5f),
                        MoistureFurnaceTemperature = GetRandomFloat(40.0f, 80.0f),
                        CuringFurnaceTemperature = GetRandomFloat(120.0f, 200.0f),
                        FactoryTemperature = GetRandomFloat(15.0f, 35.0f),
                        FactoryHumidity = GetRandomFloat(30.0f, 80.0f),
                        ProductionCount = GetRandomFloat(0, 1000),
                        DefectiveCount = GetRandomFloat(0, 50),
                        ProductionPace = GetRandomFloat(0.5f, 2.0f),
                        AccumulatedAlarms = GetRandomFloat(0, 20),
                        CreateTime = DateTime.Now.AddDays(GetRandomFloat(1f, 10f)),
                        UpdateTime = DateTime.Now.AddDays(GetRandomFloat(1f, 10f))
                    };
                    scadaReadDataList.Add(scadaReadData);
                }
               

                // 执行插入
                SqlSugarHelper.Db.Insertable(scadaReadDataList).ExecuteCommand();
            }
        }

    }

    


    /// <summary>
    /// 生成随机数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns></returns>
    private float GetRandomFloat(float min, float max)
    {
        return (float)(new Random().NextDouble() * (max - min) + min);
    }

    private void NavigateToLoginView()
    {
        try
        {
            if (_regionManager.Regions.ContainsRegionWithName("MainRegion"))
            {
                _regionManager.RequestNavigate("MainRegion", nameof(LoginView));
            }
            else
            {
                Console.WriteLine("MainRegion 未注册，导航失败");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Navigation to LoginView failed: {ex.Message}");
        }
    }
}