using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using FastReport;
using FastReport.Export.Pdf;
using GlucacxeScadaSystem.Helpers;
using GlucacxeScadaSystem.Models;
using GlucacxeScadaSystem.Services;
using MiniExcelLibs;
using Prism.Commands;
using Prism.Mvvm;

namespace GlucacxeScadaSystem.ViewModels;

public class DataQueryViewModel : BindableBase
{
    List<ScadaReadData> _scadaReadDataList = new();
    public List<ScadaReadData> ScadaReadDataList
    {
        get => _scadaReadDataList;
        private set => SetProperty(ref _scadaReadDataList, value);
    }

    private DateTime _startTime = DateTime.Now.AddDays(-30);
    public DateTime StartTime
    {
        get => _startTime;
        set => SetProperty(ref _startTime, value);
    }
    private DateTime _endTime = DateTime.Now;
    public DateTime EndTime
    {
        get => _endTime;
        set => SetProperty(ref _endTime, value);
    }


    // --- 分页相关属性 ---


    private int _currentPage = 1;  // 当前页码，默认为 1
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            // 使用 SetProperty 更新字段值，并引发 PropertyChanged 事件通知 UI
            // SetProperty 返回 true 表示值确实发生了改变
            if (SetProperty(ref _currentPage, value))
            {
                // CurrentPage 成功更改后执行的逻辑
                Search();
            }
        }
    }



    private int _totalPages = 1;  // 总页数 ，初始为1，直到数据加载后才会更新
    public int TotalPages
    {
        get => _totalPages;
        // 总页数通常是根据查询结果计算出来的，所以设置为 private set
        private set
        {
            // 当总页数变化时，翻页按钮的 CanExecute 状态可能需要更新
            // 下面命令中的 .ObservesProperty 会自动处理这个。
            if (SetProperty(ref _totalPages, value))
            {
               
            }
        }
    }

    /// <summary>
    /// 当前页大小，也就是每页显示多少条数据
    /// </summary>
    private int _pageSize = 20;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            // 如果改变 PageSize 应该重置到第一页重新加载数据
            if (SetProperty(ref _pageSize, value))
            {
                // 设置 CurrentPage = 1 会通过其 setter 触发 Search()
                CurrentPage = 1;

                // 特殊情况：如果 CurrentPage 本来就是 1，setter 不会触发 Search，需要手动调用
                if (_currentPage == 1)
                {
                    Search();
                }
            }
        }
    }

    // --- 命令 (Commands) ---


    public DelegateCommand LoadCommand { get; private set; }
    public DelegateCommand SearchCommand { get; private set; }
    public DelegateCommand ResetCommand { get; private set; }

    // ---- excel文件导出 ----
    public DelegateCommand OutPageCommand { get; private set; }
    public DelegateCommand OutAllCommand { get; private set; }
    

    /// ---- 分页 -----
    public DelegateCommand GotoFistCommand { get; private set; }
    public DelegateCommand GotoLastCommand { get; private set; }
    public DelegateCommand GotoNextCommand { get; private set; }
    public DelegateCommand GotoPreviousCommand { get; private set; }
    
        
    /// ---- 报表 ----
    public DelegateCommand DesignReportCommand { get; private set; }
    public DelegateCommand PreviewReportCommand { get; private set; }
    public DelegateCommand OutputReportCommand { get; private set; }




    public DataQueryViewModel()
    {
        LoadCommand = new DelegateCommand(ExecuteLoad);
        SearchCommand = new DelegateCommand(ExecuteSearch);
        ResetCommand = new DelegateCommand(ExecuteReset);

        OutPageCommand = new DelegateCommand(OutPage);
        OutAllCommand = new DelegateCommand(OutAll);

        DesignReportCommand = new DelegateCommand(DesignReport);
        PreviewReportCommand = new DelegateCommand(PreviewReport);
        OutputReportCommand = new DelegateCommand(OutputReport);


        // --- 初始化分页命令 ---
        // 关联命令的执行方法 (Execute...) 和可执行判断方法 (CanExecute...)
        // 使用 .ObservesProperty(() => PropertyName) (Prism 6+ 功能)
        // 让命令自动监听指定属性的变化，并在变化时重新评估 CanExecute 状态
        GotoFistCommand = new DelegateCommand(GotoFirstPage, CanGoToFirstOrPreviousPage)
            .ObservesProperty(() => CurrentPage); // 监听 CurrentPage 变化

        GotoPreviousCommand = new DelegateCommand(GotoPreviousPage, CanGoToFirstOrPreviousPage)
            .ObservesProperty(() => CurrentPage); // 监听 CurrentPage 变化

        GotoNextCommand = new DelegateCommand(GotoNextPage, CanGoToLastOrNextPage)
            .ObservesProperty(() => CurrentPage)  // 监听 CurrentPage 变化
            .ObservesProperty(() => TotalPages); // 监听 TotalPages 变化

        GotoLastCommand = new DelegateCommand(GotoLastPage, CanGoToLastOrNextPage)
            .ObservesProperty(() => CurrentPage)  // 监听 CurrentPage 变化
            .ObservesProperty(() => TotalPages); // 监听 TotalPages 变化


        // ViewModel 创建时加载初始数据
        ExecuteLoad();
    }

    /// <summary>
    /// 导出报表
    /// </summary>
    private void OutputReport()
    {
        try
        {
            var dataSet = ScadaReadDataList.ConvertToDataSet();
            var report = new Report();
            // 加载报表模板
            var path = $@"{Environment.CurrentDirectory}\Configs\report.frx";
            report.Load(path);
            report.RegisterData(dataSet);
            // 准备
            report.Prepare();
            // 导出 PDF
            var pdfExport = new PDFExport();
            pdfExport.Export(report);
            report.Dispose();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    /// <summary>
    /// 预览报表
    /// </summary>
    private void PreviewReport()
    {
        try
        {
            var dataSet = ScadaReadDataList.ConvertToDataSet();
            var report = new Report();
            // 加载报表模板
            var path = $@"{Environment.CurrentDirectory}\Configs\report.frx";
            report.Load(path);
            report.RegisterData(dataSet);

            // 准备
            report.Prepare();
            // 预览
            report.ShowPrepared();
            report.Dispose();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }




    /// <summary>
    /// 设计报表
    /// </summary>
    private void DesignReport()
    {
        try
        {
            var report = new Report();

            // 加载报表模板
            var path = $@"{Environment.CurrentDirectory}\Configs\report.frx";
            report.Load(path);

            // 报表设计   
            report.Design();

            // 导出 PDF
            var pdfExport = new PDFExport();
            pdfExport.Export(report);

            report.Dispose();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
       
    }

    private void OutAll()
    {
        var List = SqlSugarHelper.Db.Queryable<ScadaReadData>().ToList();
        SaveByMinniExcel(List);
    }

    private void OutPage()
    {
        SaveByMinniExcel<ScadaReadData>(ScadaReadDataList);
    }

    private void ExecuteLoad()
    {
        // 设置 CurrentPage = 1 会通过其 setter 自动触发 Search()
        CurrentPage = 1;
        // 如果 CurrentPage 本来就是 1，setter 可能不触发，需显式调用 Search 确保初始加载
        if (_currentPage == 1)
        {
            Search();
        }
    }


    private void ExecuteSearch()
    {

        Search();
    }

    private void ExecuteReset()
    {
        // 重置查询条件
        StartTime = DateTime.Now.AddDays(-30);
        EndTime = DateTime.Now;

        // 重置后应该回到第一页并重新搜索
        CurrentPage = 1;
        // 同样，如果 CurrentPage 本来就是 1，需要手动触发 Search
        if (_currentPage == 1)
        {
            Search();
        }
    }


    private void GotoFirstPage()
    {
        CurrentPage = 1; // 设置页码，setter 会自动触发 Search
    }


    private void GotoPreviousPage()
    {
       
        if (CanGoToFirstOrPreviousPage()) // 检查是否可以执行
        {
            CurrentPage--; // 设置页码，setter 会自动触发 Search
        }
    }

    private void GotoNextPage()
    {
        if (CanGoToLastOrNextPage()) // 检查是否可以执行
        {
            CurrentPage++; // 设置页码，setter 会自动触发 Search
        }
    }

    private void GotoLastPage()
    {
        if (CanGoToLastOrNextPage()) // 检查是否可以执行
        {
            CurrentPage = TotalPages; // 设置页码，setter 会自动触发 Search
        }
    }


    // --- 命令可执行判断方法 (CanExecute Methods) ---

    private bool CanGoToFirstOrPreviousPage()
    {
        bool canExecute = CurrentPage > 1; // 只有当前页大于1时才能往前翻
        return canExecute;
    }

    private bool CanGoToLastOrNextPage()
    {
        bool canExecute = CurrentPage < TotalPages; // 只有当前页小于总页数时才能往后翻
        return canExecute;
    }



    // --- 数据获取逻辑 ---
    private void Search()
    {

        // 1. 输入验证
        if (StartTime > EndTime)
        {
            MessageBox.Show("开始时间不能大于结束时间！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            return; // 停止搜索
        }

        // 2. 准备查询参数
        int totalNumber = 0; // 用于接收查询返回的总记录数
        List<ScadaReadData> results = new List<ScadaReadData>(); // 用于存储查询结果
        int pageToFetch = CurrentPage; // 使用当前的页码进行查询

        // （可选）安全起见，限制页码范围
        if (pageToFetch < 1) { pageToFetch = 1; }
        // 这里不能轻易用 TotalPages 来限制上限，因为 TotalPages 可能是上次查询的旧值

        try
        {
            // 3. 执行数据库查询 
            results = SqlSugarHelper.Db.Queryable<ScadaReadData>()
                .Where(x => x.CreateTime >= StartTime && x.CreateTime <= EndTime) // 应用时间范围过滤条件
                .OrderBy(x => x.CreateTime) // **极其重要**: 分页查询必须指定排序(OrderBy)，否则每次查询的顺序可能不同，导致分页混乱。请替换为你需要排序的列。
                .ToPageList(pageToFetch, PageSize, ref totalNumber); // 执行分页查询


            // 4. 更新 ViewModel 属性
            ScadaReadDataList = results; // 更新绑定到 DataGrid 的数据列表
            TotalPages = (totalNumber == 0) ? 1 : (int)Math.Ceiling((double)totalNumber / PageSize); // 根据总记录数和页面大小计算总页数（至少1页）

            // 5. 处理边界情况：如果数据变化导致当前页码失效
            // (例如，删除了最后一页的最后一条数据，导致总页数减少)
            if (CurrentPage > TotalPages && TotalPages > 0) // 如果当前页超出了新的总页数
            {
                // 跳转到新的最后一页。这会通过 CurrentPage 的 setter 再次触发 Search()
                CurrentPage = TotalPages;
            }
            // 如果总页数变为 0 或 1，确保当前页也是 1
            else if (TotalPages <= 1 && CurrentPage != 1)
            {
                // 可以直接设置 CurrentPage = 1，让 setter 处理后续逻辑
                CurrentPage = 1;
            }
        }
        catch (Exception ex)
        {
            // 6. 错误处理
            System.Diagnostics.Debug.WriteLine($"搜索过程中发生错误: {ex}");
            MessageBox.Show($"查询数据时出错: {ex.Message}", "数据库错误", MessageBoxButton.OK, MessageBoxImage.Error);
            // 出错时重置状态
            ScadaReadDataList = new List<ScadaReadData>(); // 清空列表
            TotalPages = 1; // 重置总页数
                            // 也可以考虑重置 CurrentPage，但要小心避免立即再次触发失败的 Search
            if (_currentPage != 1) // 检查后台字段
            {
                _currentPage = 1; // 直接修改后台字段
                RaisePropertyChanged(nameof(CurrentPage)); // 手动通知 UI 更新页码显示
            }
        }
        finally
        {
            // 7. 确保命令的 CanExecute 状态被更新
            System.Diagnostics.Debug.WriteLine("Search 方法执行完毕。");
        }
    }


    private void SaveByMinniExcel<T>(List<T> list)
    {
        if (list.Count < 1)
        {
            return;
        }

        var rootPath = AppDomain.CurrentDomain.BaseDirectory+"\\Excels\\";
        if (!Directory.Exists(rootPath))
        {
            Directory.CreateDirectory(rootPath);
        }

        var excelPath = rootPath + DateTime.Now.ToString("yyyyMMddHHmmss")+ ".xlsx";

        try
        {
            MiniExcel.SaveAs(excelPath, list);
            MessageBox.Show($"导出文件成功{excelPath}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"导出文件异常{ex.Message}");
        }
    }

}