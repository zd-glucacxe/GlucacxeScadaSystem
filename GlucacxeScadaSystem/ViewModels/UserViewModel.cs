using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using AngleSharp.Dom;
using GlucacxeScadaSystem.Helpers;
using GlucacxeScadaSystem.Models;
using GlucacxeScadaSystem.Services;
using GlucacxeScadaSystem.Views;
using Masuit.Tools;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Dialog = GlucacxeScadaSystem.UserControls.Dialog;

namespace GlucacxeScadaSystem.ViewModels;

public class UserViewModel : BindableBase
{
    public UserSession _userSession { get; }

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
        _userSession = userSession;
        LoadCommand = new DelegateCommand(Load);
        SearchUserCommand = new DelegateCommand(SearchUser);
        AddUserCommand = new DelegateCommand(AddUser);
        DeleteUserCommand = new DelegateCommand<User>(DeleteUser);
        EditUserCommand = new DelegateCommand<User>(EditUser);

    }

    /// <summary>
    /// 修改用户
    /// </summary>
    /// <param name="user"></param>
    private async void EditUser(User user)
    {
        if (_userSession.CurrentUser.Role != 0)
        {
            _userSession.ShowMessageBox("非管理员，无权限");
            return;
        }

        //if (UserSession.CurrentUser.UserName == user.UserName)
        //{
        //    _userSession.ShowMessageBox("你不能修改自己！");
        //    return;
        //}

        // 编辑用户，由于是引用类型，所以涉及到深拷贝、浅拷贝
        var userClone = user.DeepClone();
        var view = new UserOperateView() { DataContext = new UpdateCommViewModel<User>(userClone) };
        var res = (bool)await DialogHost.Show(view, "ShellDialog");

        // 弹窗点击确定 
        if (res)
        {
            // 编辑用户
            user.UserName = userClone.UserName;
            user.PassWord = userClone.PassWord;
            user.Role = userClone.Role;
            user.UpdateTime = DateTime.Now;
            int count = await SqlSugarHelper.Db.Updateable(user).ExecuteCommandAsync();

            if (count > 0)
            {
                SearchUser();
                _userSession.ShowMessageBox("修改成功！");
            }
        }

    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="user"></param>
    private async void DeleteUser(User user)
    {
        if (_userSession.CurrentUser.Role != 0)
        {
            _userSession.ShowMessageBox("非管理员，无权限");
            return;
        }

        if (_userSession.CurrentUser.UserName == user.UserName)
        {
            _userSession.ShowMessageBox("不能删除自己！");
            return;
        }

        var res = (bool) await DialogHost.Show(new Dialog("是否删除", MessageBoxButton.YesNo), "ShellDialog");

      
        if (res)
        {
            int count = SqlSugarHelper.Db.Deleteable<User>().Where(it => it.Id == user.Id).ExecuteCommand();
            if (count > 0)
            {
                SearchUser();
                _userSession.ShowMessageBox("删除成功");
            }
        }
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    private async void AddUser()
    {
        try
        {
            if (_userSession.CurrentUser.Role != 0)
            {
                _userSession.ShowMessageBox("非管理员，无权限");
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
                    _userSession.ShowMessageBox("添加成功！");
                }
            }
        }
        catch (Exception ex)
        {
            _userSession.ShowMessageBox("添加失败！" + ex.Message);
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