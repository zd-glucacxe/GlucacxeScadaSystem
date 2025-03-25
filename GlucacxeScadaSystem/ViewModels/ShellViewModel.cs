using GlucacxeScadaSystem.EventAggregator;
using GlucacxeScadaSystem.Models;
using GlucacxeScadaSystem.Views;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;

namespace GlucacxeScadaSystem.ViewModels;

public class ShellViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private readonly IEventAggregator _eventAggregator;

    public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
    {
        InitData();
        _regionManager = regionManager;
        NavigateToLoginView();
        _eventAggregator = eventAggregator;

        _eventAggregator.GetEvent<ChangUserEvent>().Subscribe(RedirectToLoginView);
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
    

        if (false)
        {
            // 第一次建表 则为true 随后为false
            if (false)
            {
                // 建库
                SqlSugarHelper.Db.DbMaintenance.CreateDatabase();
                // 建表
                SqlSugarHelper.Db.CodeFirst.InitTables(typeof(User));

            }
            // 插入数据 root user user1
            if (SqlSugarHelper.Db.Queryable<User>().Count() == 0)
            {
                var userList = new List<User>();
                userList.Add(new User() { UserName = "root", PassWord = "root", Role = 0 });
                userList.Add(new User() { UserName = "user", PassWord = "user", Role = 1 });
                userList.Add(new User() { UserName = "user1", PassWord = "user1", Role = 1 });

                // 执行插入
                SqlSugarHelper.Db.Insertable(userList).ExecuteCommand();
            }
        }
        

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