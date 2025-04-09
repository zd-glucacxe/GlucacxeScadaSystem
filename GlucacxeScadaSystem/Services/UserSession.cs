using System.Windows;
using GlucacxeScadaSystem.Models;
using GlucacxeScadaSystem.UserControls;
using MaterialDesignThemes.Wpf;
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


    public void ShowMessageBox(string content, MessageBoxButton button = MessageBoxButton.OK)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            DialogHost.Show(new Dialog(content, button), "ShellDialog");
        });

    }
}