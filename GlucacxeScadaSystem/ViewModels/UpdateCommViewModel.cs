using Prism.Mvvm;

namespace GlucacxeScadaSystem.ViewModels;

public class UpdateCommViewModel<T> : BindableBase where T : class
{
    public T Entity { get; set; }   

    public UpdateCommViewModel(T entity)
    {
        Entity = entity;
    }   
   
}