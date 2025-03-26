using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using GlucacxeScadaSystem.Models;
using GlucacxeScadaSystem.Services;
using GlucacxeScadaSystem.Views;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace GlucacxeScadaSystem.ViewModels;

public class UserViewModel : BindableBase
{
    public UserSession UserSession { get; }

    List<User> _userList = new();
    public List<User> UserList
    {
        get => _userList;
        set => SetProperty(ref _userList, value);
    }


    public DelegateCommand LoadCommand { get; private set; }
    
    public DelegateCommand SearchUserCommand { get; private set; }
    public DelegateCommand<User> DeleteUserCommand { get; private set; }
    public DelegateCommand AddUserCommand { get; private set; }
    public DelegateCommand<User> EditUserCommand { get; private set; }



    public UserViewModel(UserSession userSession)
    {
        UserSession = userSession;
        LoadCommand = new DelegateCommand(Load);
        SearchUserCommand = new DelegateCommand(SearchUser);
        AddUserCommand = new DelegateCommand(AddUser);


    }

    /// <summary>
    /// 添加用户
    /// </summary>
    private async void AddUser()
    {
        try
        {
            if (UserSession.CurrentUser.Role != 0)
            {
                MessageBox.Show("非管理员，无权限");
                return;
            }
            // 给弹窗添加上下文 其中Entity 就是User
            var entity = new User();
            var view = new UserOperateView() { DataContext = new UpdateCommViewModel<User>(entity) };
            var res = (bool)await DialogHost.Show(view, "ShellDialog");

            // 弹窗点击确定 
            if (res)
            {
                int count = await SqlSugarHelper.Db.Insertable(entity).ExecuteCommandAsync();
                if (count > 0)
                {
                    SearchUser();
                    MessageBox.Show("添加成功！");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("添加失败！" + ex.Message);
        }
        
    }
    


    /// <summary>
    /// 窗体加载实现第一次查询
    /// </summary>
    private void Load()
    {
        SearchUser();
    }

    private void SearchUser()
    {
        UserList = SqlSugarHelper.Db.Queryable<User>().ToList();
    }
}