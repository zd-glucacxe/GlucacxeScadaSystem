using GlucacxeScadaSystem.Views;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace GlucacxeScadaSystem.ViewModels;

public class ShellViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;

    public ShellViewModel(IRegionManager regionManager)
    {
        _regionManager = regionManager;
        NavigateToLoginView();
    }

    private void NavigateToLoginView()
    {
        try
        {
            _regionManager.RequestNavigate("MainRegion", nameof(LoginView));
            Console.WriteLine("Navigation to LoginView requested successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Navigation to LoginView failed: {ex.Message}");
        }
    }
}