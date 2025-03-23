﻿using GlucacxeScadaSystem.EventAggregator;
using GlucacxeScadaSystem.Models;
using GlucacxeScadaSystem.Services;
using Prism.Events;
using Prism.Mvvm;

namespace GlucacxeScadaSystem.ViewModels;

public class MainViewModel : BindableBase
{
    public UserSession UserSession { get; }

    private readonly IEventAggregator _eventAggregator;

    private User _currentUser;
    public User CurrentUser
    {
        get => _currentUser;
        set => SetProperty(ref _currentUser, value);
    }

    public MainViewModel(UserSession userSession, IEventAggregator eventAggregator)
    {
        UserSession = userSession;
        _eventAggregator = eventAggregator;

        // 订阅 LoginEvent;
        _eventAggregator.GetEvent<LoginEvent>().Subscribe(OnUserLoggedIn);
    }

    private void OnUserLoggedIn(User user)
    {
        CurrentUser = user;
    }
}