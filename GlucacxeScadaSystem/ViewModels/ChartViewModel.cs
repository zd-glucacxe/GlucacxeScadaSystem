using System;
using System.Collections.Generic;
using System.Linq;
using FastReport.DevComponents.DotNetBar.Controls;
using GlucacxeScadaSystem.Models;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using ScottPlot;
using ScottPlot.WPF;

namespace GlucacxeScadaSystem.ViewModels;

public class ChartViewModel : BindableBase
{
    private WpfPlot _plot;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger(); // 直接使用 

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

    public DelegateCommand SearchCommand { get; private set; }

    public ChartViewModel()
    {
        SearchCommand = new DelegateCommand(Search);
    }

    private void Search()
    {
        if (EndTime < StartTime) return;

        try
        {
            // 1. 清除所有的数据
            _plot.Plot.Clear();

            // 2. 查询数据库中的数据
            var data = SqlSugarHelper.Db.Queryable<ScadaReadData>()
                .Where(x => x.CreateTime >= StartTime && x.CreateTime <= EndTime)
                .OrderBy(x => x.CreateTime, SqlSugar.OrderByType.Asc)
                               .ToList();
            // 3. 判断数据受否为空，如果未空，直接返回
            if (data.Count == 0) return;

            // 4. 将数据添加到 plot 中
            var DegreasingSprayPumpPressure = data.Select(x => x.DegreasingSprayPumpPressure).ToArray();
            var DegreasingPhValue = data.Select(x => x.DegreasingPhValue).ToArray();
            var RoughWashSprayPumpPressure = data.Select(x => x.RoughWashSprayPumpPressure).ToArray();
            var PhosphatingSprayPumpPressure = data.Select(x => x.PhosphatingSprayPumpPressure).ToArray();
            //var PhosphatingPhValue = data.Select(x => x.PhosphatingPhValue).ToArray();
            //var FineWashSprayPumpPressure = data.Select(x => x.FineWashSprayPumpPressure).ToArray();
            //var MoistureFurnaceTemperature = data.Select(x => x.MoistureFurnaceTemperature).ToArray();
            //var CuringFurnaceTemperature = data.Select(x => x.CuringFurnaceTemperature).ToArray();
            //var FactoryTemperature = data.Select(x => x.FactoryTemperature).ToArray();
            //var FactoryHumidity = data.Select(x => x.FactoryHumidity).ToArray();
            //var ProductionCount = data.Select(x => x.ProductionCount).ToArray();
            //var DefectiveCount = data.Select(x => x.DefectiveCount).ToArray();
            //var ProductionPace = data.Select(x => x.ProductionPace).ToArray();
            //var AccumulatedAlarms = data.Select(x => x.AccumulatedAlarms).ToArray();

            // 5. 设置线条样式
            List<LinePattern> paList = new List<LinePattern>();
            paList.AddRange(LinePattern.GetAllPatterns());

            // 6. 添加数据
            var sg1 = _plot.Plot.Add.Signal(DegreasingSprayPumpPressure);
            sg1.LegendText = "DegreasingSprayPumpPressure";
            sg1.LinePattern = paList[0];
            
            var sg2 = _plot.Plot.Add.Signal(DegreasingPhValue);
            sg2.LegendText = "DegreasingPhValue";
            sg2.LinePattern = paList[1];

            var sg3 = _plot.Plot.Add.Signal(RoughWashSprayPumpPressure);
            sg3.LegendText = "RoughWashSprayPumpPressure";
            sg3.LinePattern = paList[2];

            var sg4 = _plot.Plot.Add.Signal(PhosphatingSprayPumpPressure);
            sg4.LegendText = "PhosphatingSprayPumpPressure";
            sg4.LinePattern = paList[3];

            //var sg5 = _plot.Plot.Add.Signal(PhosphatingPhValue);
            //sg5.LegendText = "PhosphatingPhValue";
            //sg5.LinePattern = paList[4];

            //var sg6 = _plot.Plot.Add.Signal(FineWashSprayPumpPressure);
            //sg6.LegendText = "FineWashSprayPumpPressure";
            //sg6.LinePattern = paList[5];

            //var sg7 = _plot
            //    .Plot.Add.Signal(MoistureFurnaceTemperature);
            //sg7.LegendText = "MoistureFurnaceTemperature";
            //sg7.LinePattern = paList[6];

            //var sg8 = _plot.Plot.Add.Signal(CuringFurnaceTemperature);
            //sg8.LegendText = "CuringFurnaceTemperature";
            //sg8.LinePattern = paList[7];

            //var sg9 = _plot.Plot.Add.Signal(FactoryTemperature);
            //sg9.LegendText = "FactoryTemperature";
            //sg9.LinePattern = paList[8];

            //var sg10 = _plot.Plot.Add.Signal(FactoryHumidity);
            //sg10.LegendText = "FactoryHumidity";
            //sg10.LinePattern = paList[9];

            //var sg11 = _plot.Plot.Add.Signal(ProductionCount);
            //sg11.LegendText = "ProductionCount";
            //sg11.LinePattern = paList[10];

            //var sg12 = _plot.Plot.Add.Signal(DefectiveCount);
            //sg12.LegendText = "DefectiveCount";
            //sg12.LinePattern = paList[11];

            //var sg13 = _plot.Plot.Add.Signal(ProductionPace);
            //sg13.LegendText = "ProductionPace";
            //sg13.LinePattern = paList[12];

            //var sg14 = _plot.Plot.Add.Signal(AccumulatedAlarms);
            //sg14.LegendText = "AccumulatedAlarms";
            //sg14.LinePattern = paList[13];

            _plot.Plot.Axes.AutoScale();

            // 7. 缩放
            _plot.Plot.ScaleFactor = 2;

            // 8. 刷新
            _plot.Refresh();
        }

        catch (Exception ex)
        {
            _logger.Error(ex);
        }

    }


    public void InitPlot(WpfPlot plot)
    {
        _plot = plot;
        ConfigurePlot(); // 先配置基本样式
        Search();        // 然后加载初始数据
    }

    private void ConfigurePlot()
    {
        if (_plot == null) return;

        // 配置图表样式
        _plot.Plot.Title("ScadaReadData");
        _plot.Plot.XLabel("Point");
        _plot.Plot.YLabel("Value");
        // 显示图例
        _plot.Plot.ShowLegend();

        // 刷新图表以应用样式
        _plot.Refresh();

        //Search();
    }


}