using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using GlucacxeScadaSystem.ViewModels;

namespace GlucacxeScadaSystem.Views
{
    /// <summary>
    /// ChartView.xaml 的交互逻辑
    /// </summary>
    public partial class ChartView : UserControl
    {
        public ChartView()
        {
            InitializeComponent();
            Loaded += ChartView_Loaded;
        }


        private void ChartView_Loaded(object sender, RoutedEventArgs e)
        {
            // 从 ViewModelLocator 设置的 DataContext 获取 ViewModel
            // 使用 'as' 进行安全转换，或在转换前检查类型
            if (this.DataContext is ChartViewModel viewModel)
            {
                // 检查 WpfPlot 控件本身是否已准备好 (InitializeComponent 之后应该好了)
                if (WpfPlot != null)
                {
                    // 将 WpfPlot 控件传递给 ViewModel
                    viewModel.InitPlot(WpfPlot);
                }
                else
                {
                    // 如果 WpfPlot 意外为 null，记录错误
                    Debug.WriteLine("错误: ChartView_Loaded 中的 WpfPlot 控件为 null。");
                    // 如果可以在此处访问 NLog 或通过 ViewModel，也可以使用它们记录日志
                }
            }
            else
            {
                // 如果 DataContext 不是预期的 ViewModel，记录错误
                // 这可能表示 ViewModelLocator 设置或命名约定有问题
                Debug.WriteLine(
                    $"错误: ChartView_Loaded 中的 DataContext 不是 ChartViewModel。实际类型: {this.DataContext?.GetType().Name ?? "null"}");
            }
        }
    }
}
