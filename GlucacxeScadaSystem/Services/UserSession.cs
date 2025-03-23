using GlucacxeScadaSystem.Models;
using Prism.Mvvm;

namespace GlucacxeScadaSystem.Services;

public class UserSession : BindableBase
{
    private User _user = new User(){UserName = "test", PassWord = "test"};

    public User CurrentUser
    {
        get => _user;
        set => SetProperty(ref _user, value);
    }
}