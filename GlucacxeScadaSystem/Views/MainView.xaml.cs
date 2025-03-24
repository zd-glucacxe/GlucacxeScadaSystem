using GlucacxeScadaSystem.EventAggregator;
using Prism.Events;
using System;
using System.Reflection;
using System.Windows.Controls;
using GlucacxeScadaSystem.Models;
using Menu = GlucacxeScadaSystem.Models.Menu;
using Prism.Regions;

namespace GlucacxeScadaSystem.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl
    {
        private readonly IRegionManager _regionManager;

        private readonly IEventAggregator _eventAggregator;

        public MainView(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator; 
            InitializeComponent();
            
            InitialSubscribe();

            // 直接跳转到 IndexView
            var indexMenu = new Menu { View = "IndexView" };
            OnSelectMenuItem(indexMenu);
        }

        private void InitialSubscribe()
        {
            _eventAggregator.GetEvent<RegionEvent>().Subscribe(OnSelectMenuItem);
        }

        private void OnSelectMenuItem(Menu menu)
        {
            // 根据 menu.View 动态获取视图所在的类型，并进行导航
            Assembly assembly = Assembly.GetExecutingAssembly();
            var type = assembly.GetType($"{assembly.GetName().Name}.Views.{menu.View}");
            if (type != null)
            {
                Page.Content = Activator.CreateInstance(type);
            }
        }
    }
}