using System;
using System.Collections.Generic;
using System.Windows.Documents;
using GlucacxeScadaSystem.EventAggregator;
using GlucacxeScadaSystem.Helpers;
using GlucacxeScadaSystem.Models;
using GlucacxeScadaSystem.Services;
using NLog;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SqlSugar;

namespace GlucacxeScadaSystem.ViewModels;

public class MainViewModel : BindableBase
{
    // 测试日志功能和参数获取
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly RootParam _rootParam;


    public UserSession UserSession { get; }

    private readonly IEventAggregator _eventAggregator;

    public DelegateCommand<Menu> NavigationCommand { get; private set; }

    public DelegateCommand ChangeUserCommand { get; private set; }
    

    private User _currentUser;
    public User CurrentUser
    {
        get => _currentUser;
        set => SetProperty(ref _currentUser, value);
    }

    public List<Menu> MenuEntities { get; set; } = new();

   
    public MainViewModel(
        UserSession userSession,
        IEventAggregator eventAggregator,
        RootParam rootParam
        
        )
    {
        UserSession = userSession;
        _eventAggregator = eventAggregator;
        _rootParam = rootParam;
        

        // 订阅 LoginEvent;
        _eventAggregator.GetEvent<LoginEvent>().Subscribe(OnUserLoggedIn);

        InitData();
            
        NavigationCommand = new DelegateCommand<Menu>(Navigation);

        ChangeUserCommand = new DelegateCommand(ChangeUser);

       
    }

    /// <summary>
    /// 切换用户
    /// </summary>
    private void ChangeUser()
    {
        _eventAggregator.GetEvent<ChangUserEvent>().Publish();
    }


    /// <summary>
    /// 按钮被点击导航到对应的页面
    /// </summary>
    /// <param name="menu"></param>
    private void Navigation(Menu menu)
    {
        _eventAggregator.GetEvent<RegionEvent>().Publish(menu);
    }

    private void InitData()
    {
        MenuEntities = SqlSugarHelper.Db.Queryable<Menu>().ToList();

        // 测试
        //_logger.Info("测试日志功能");
        //_logger.Trace($"PLC的IP地址是：{_rootParam.PlcParam.PlcIp}");

    }

    private void OnUserLoggedIn(User user)
    {
        CurrentUser = user;

       
    }

}