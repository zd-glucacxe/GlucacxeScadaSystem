using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GlucacxeScadaSystem.UserControls
{
    /// <summary>
    /// StatusIndicatorControl.xaml 的交互逻辑
    /// </summary>
    public partial class StatusIndicatorControl : UserControl
    {
        public StatusIndicatorControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 按钮背景色
        /// </summary>
        public static readonly DependencyProperty ButtonBackgroundProperty = DependencyProperty.Register(
            nameof(ButtonBackground), typeof(Brush), typeof(StatusIndicatorControl), new PropertyMetadata(Brushes.Green));

        public Brush ButtonBackground
        {
            get { return (Brush)GetValue(ButtonBackgroundProperty); }
            set { SetValue(ButtonBackgroundProperty, value); }
        }

        /// <summary>
        /// 是否运行状态显示
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
            nameof(IsIndeterminate), typeof(bool), typeof(StatusIndicatorControl), new PropertyMetadata(false));

        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        /// <summary>
        /// 状态文本
        /// </summary>
        public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register(
            nameof(StatusText), typeof(string), typeof(StatusIndicatorControl), new PropertyMetadata(string.Empty));

        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }
    }
}
