using System.Collections.Generic;
using System.Windows.Documents;
using GlucacxeScadaSystem.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace GlucacxeScadaSystem.ViewModels;

public class UserViewModel : BindableBase
{

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



    public UserViewModel()
    {
        LoadCommand = new DelegateCommand(Load);
        SearchUserCommand = new DelegateCommand(SearchUser);
        
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