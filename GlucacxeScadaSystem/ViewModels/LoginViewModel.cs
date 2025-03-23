using System.Collections.Generic;
using System.Windows;
using GlucacxeScadaSystem.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace GlucacxeScadaSystem.ViewModels;

public class LoginViewModel : BindableBase
{
    private string _username;
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    private string _password;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public DelegateCommand LoginCommand { get; }
     
    public LoginViewModel()
    {
        InitData();

        LoginCommand = new DelegateCommand(OnLogin);
    }

    /// <summary>
    ///  数据库初始化 ,创建表及一些数据
    /// </summary>
    private void InitData()
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

    private void OnLogin()
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
        {
            MessageBox.Show("用户名或密码为空！");
        }

        // 查询登录
        var userRes = SqlSugarHelper.Db.Queryable<User>()
            .Where(x => x.UserName == Username && x.PassWord == Password).ToList();

        if (userRes.Count == 0)
        {
            MessageBox.Show("用户名或密码错误！");
        }
        else
        {
            MessageBox.Show("登录成功！");
        } 
    }
}