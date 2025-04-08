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
    /// PaginationControl.xaml 的交互逻辑
    /// </summary>
    public partial class PaginationControl : UserControl
    {
        public PaginationControl()
        {
            InitializeComponent();
        }


        #region 定义一系列附加属性，挂接到前台应用

        /// <summary>
        /// 当前页
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register(
            nameof(CurrentPage), typeof(int), typeof(PaginationControl), new PropertyMetadata(1));
        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        /// <summary>
        /// 总页码
        /// </summary>
        public static readonly DependencyProperty TotalPagesProperty = DependencyProperty.Register(
            nameof(TotalPages), typeof(int), typeof(PaginationControl), new PropertyMetadata(1));
        public int TotalPages
        {
            get { return (int)GetValue(TotalPagesProperty); }
            set { SetValue(TotalPagesProperty, value); }
        }


        #region 命令

        /// <summary>
        /// 去首页命令
        /// </summary>
        public static readonly DependencyProperty GotoFirstPageCommandProperty = DependencyProperty.Register(
            nameof(GotoFirstPageCommand), typeof(ICommand), typeof(PaginationControl));
        public ICommand GotoFirstPageCommand
        {
            get { return (ICommand)GetValue(GotoFirstPageCommandProperty); }
            set { SetValue(GotoFirstPageCommandProperty, value); }
        }

        /// <summary>
        /// 上一页命令
        /// </summary>
        public static readonly DependencyProperty GotoPreviousPageCommandProperty = DependencyProperty.Register(
            nameof(GotoPreviousPageCommand), typeof(ICommand), typeof(PaginationControl));
        public ICommand GotoPreviousPageCommand
        {
            get { return (ICommand)GetValue(GotoPreviousPageCommandProperty); }
            set { SetValue(GotoPreviousPageCommandProperty, value); }
        }

        /// <summary>
        /// 下一页命令
        /// </summary>
        public static readonly DependencyProperty GotoNextPageCommandProperty = DependencyProperty.Register(
            nameof(GotoNextPageCommand), typeof(ICommand), typeof(PaginationControl));
        public ICommand GotoNextPageCommand
        {
            get { return (ICommand)GetValue(GotoNextPageCommandProperty); }
            set { SetValue(GotoNextPageCommandProperty, value); }
        }

        /// <summary>
        /// 去尾页命令
        /// </summary>
        public static readonly DependencyProperty GotoLastPageCommandProperty = DependencyProperty.Register(
            nameof(GotoLastPageCommand), typeof(ICommand), typeof(PaginationControl));
        public ICommand GotoLastPageCommand
        {
            get { return (ICommand)GetValue(GotoLastPageCommandProperty); }
            set { SetValue(GotoLastPageCommandProperty, value); }
        }

        #endregion

        #endregion



    }
}
