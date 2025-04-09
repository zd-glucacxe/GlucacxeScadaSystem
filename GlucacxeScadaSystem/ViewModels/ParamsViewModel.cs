using GlucacxeScadaSystem.Models;
using Prism.Mvvm;

namespace GlucacxeScadaSystem.ViewModels;

public class ParamsViewModel : BindableBase
{
    public RootParam RootParam { get; }

    public ParamsViewModel(RootParam rootParam)
    {
        RootParam = rootParam;
    }
}