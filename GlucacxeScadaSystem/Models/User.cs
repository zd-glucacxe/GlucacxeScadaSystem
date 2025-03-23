using SqlSugar;

namespace GlucacxeScadaSystem.Models;

[SugarTable("user")]
public class User : EntityBase
{
    private string _username;
    
    public string UserName
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    private string _password;

    public string PassWord
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }


    /// <summary>
    ///  0 - 管理员, 1 - 普通用户
    /// </summary>
    private int _role;
    public int Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }
}