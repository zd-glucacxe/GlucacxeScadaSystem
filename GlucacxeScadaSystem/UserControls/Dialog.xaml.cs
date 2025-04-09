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

        /// <summary>
        /// 选择性传入一个按钮类型，用于控制对话框上显示哪些按钮
        /// </summary>
        /// <param name="content">对话框显示的内容</param>
        /// <param name="button">指定要显示哪些按钮</param>
        public Dialog(string content, MessageBoxButton button = MessageBoxButton.OK)
        {
            InitializeComponent();
            TextBlock.Text = content;

            if (button == MessageBoxButton.YesNo)
            {
                StackPanelYesOrNo.Visibility = Visibility.Visible;
            }
            else
            {
                StackPanelOk.Visibility = Visibility.Visible;
            }
        }
    }
}