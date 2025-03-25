using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using GlucacxeScadaSystem.EventAggregator;
using GlucacxeScadaSystem.Models;
using GlucacxeScadaSystem.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace GlucacxeScadaSystem.ViewModels;

public class LoginViewModel : BindableBase
{
    public UserSession UserSession { get; }


    private readonly IEventAggregator _eventAggregator;
    private readonly IRegionManager _regionManager; // ① 添加 RegionManager 进行导航

    public DelegateCommand LoginCommand { get; }
     
    public LoginViewModel(UserSession userSession, IEventAggregator eventAggregator, IRegionManager IregionManager)
    {

        //InitData();
        UserSession = userSession;
        _eventAggregator = eventAggregator;
        _regionManager = IregionManager;  // ② 依赖注入 RegionManager

        LoginCommand = new DelegateCommand(OnLogin);

       
    }

 


    private string _username = "user";
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    private string _password = "user";
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    

    private void OnLogin()
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
        {
            MessageBox.Show("用户名或密码为空！");
            return;
        }

        // ③ 查询数据库验证用户
        var userRes = SqlSugarHelper.Db.Queryable<User>()
            .Where(x => x.UserName == Username && x.PassWord == Password).ToList();

        if (userRes.Count == 0)
        {
            MessageBox.Show("用户名或密码错误！");
        }
        else
        {
            // 将登录的用户信息存入UserSession
            // ④ 存储登录用户信息
            UserSession.CurrentUser = userRes[0];
            
            // 跳转到首页
            // ⑤ 发布事件，通知 MainViewModel 订阅者
            _eventAggregator.GetEvent<LoginEvent>().Publish(userRes[0]);

            // ⑥ 跳转到 MainView
            _regionManager.RequestNavigate("MainRegion", "MainView");
        }
    }
}