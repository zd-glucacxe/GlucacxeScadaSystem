using MaterialDesignThemes.Wpf;
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
    /// Dialog.xaml 的交互逻辑
    /// </summary>
    public partial class Dialog : UserControl
    {
        public Dialog()
        {
            InitializeComponent();
        }

        public Dialog(string content, MessageBoxButton button = MessageBoxButton.OK)
        {
            InitializeComponent();
            TextBlock.Text = content;

            ButtonPanel.Children.Clear();

            if (button == MessageBoxButton.OK)
            {
                ButtonPanel.Children.Add(CreateButton("确定", true));
            }
            else if (button == MessageBoxButton.OKCancel)
            {
                ButtonPanel.Children.Add(CreateButton("确定", true));
                ButtonPanel.Children.Add(CreateButton("取消", false));
            }
            else if (button == MessageBoxButton.YesNo)
            {
                ButtonPanel.Children.Add(CreateButton("是", true));
                ButtonPanel.Children.Add(CreateButton("否", false));
            }
            else if (button == MessageBoxButton.YesNoCancel)
            {
                ButtonPanel.Children.Add(CreateButton("是", true));
                ButtonPanel.Children.Add(CreateButton("否", false));
                ButtonPanel.Children.Add(CreateButton("取消", null));
            }
        }

        private Button CreateButton(string text, object result)
        {
            return new Button
            {
                Content = text,
                Margin = new Thickness(5, 0, 0, 0),
                Style = (Style)FindResource("MaterialDesignContainedButton"),
                Command = DialogHost.CloseDialogCommand,
                CommandParameter = result
            };
        }

    }
}
