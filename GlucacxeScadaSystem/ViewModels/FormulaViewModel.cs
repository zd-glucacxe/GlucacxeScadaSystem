using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using GlucacxeScadaSystem.Helpers;
using GlucacxeScadaSystem.Models;
using GlucacxeScadaSystem.Services;
using Masuit.Tools;
using NLog;
using Prism.Commands;
using Prism.Mvvm;

namespace GlucacxeScadaSystem.ViewModels;

public class FormulaViewModel : BindableBase
{
    private readonly GlobalConfig _globalConfig;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger(); // 直接使用 
    private readonly UserSession _userSession;
    /// <summary>
    /// 配方列表
    /// </summary>
    private ObservableCollection<FormulaEntity> _formulaList = new();
    public ObservableCollection<FormulaEntity> FormulaList
    {
        get => _formulaList;
        set => SetProperty(ref _formulaList, value);
    }

    /// <summary>
    /// 选中的配方
    /// </summary>
    private FormulaEntity _selectFormula;
    public FormulaEntity SelectFormula
    {
        get => _selectFormula;
        set => SetProperty(ref _selectFormula, value);
    }

    /// <summary>
    /// 当前的配方
    /// </summary>
    private FormulaEntity _currentFormula;
    public FormulaEntity CurrentFormula
    {
        get => _currentFormula;
        set => SetProperty(ref _currentFormula, value);
    }

    public DelegateCommand<FormulaEntity> SelectFormulaCommand { get; private set; }
    
    public DelegateCommand QueryFormulaCommand { get; private set; }
    public DelegateCommand CreateFormulaCommand { get; private set; }
    public DelegateCommand SaveFormulaCommand { get; private set; }
    public DelegateCommand DeleteFormulaCommand { get; private set; }
    public DelegateCommand DownloadFormulaCommand { get; private set; }

    public FormulaViewModel(UserSession userSession, GlobalConfig globalConfig)
    {
        _userSession = userSession;
        _globalConfig = globalConfig;

        QueryFormula();
        SelectFormulaCommand = new DelegateCommand<FormulaEntity>(SelectedFormula);

        QueryFormulaCommand = new DelegateCommand(QueryFormula);
        CreateFormulaCommand = new DelegateCommand(CreateFormula);
        SaveFormulaCommand = new DelegateCommand(SaveFormula);
        DeleteFormulaCommand = new DelegateCommand(DeleteFormula);
        DownloadFormulaCommand = new DelegateCommand(DownloadFormula);
    }

    private void SelectedFormula(FormulaEntity formula)
    {
        // 1. 更新选中状态，将配方中的所有配方都设置为未选中
        FormulaList.ForEach(x => x.IsSelected = false);
        // 2. 将当前选中的配方设置为选中
        formula.IsSelected = true;
        // 3. 选中的配方
        SelectFormula = formula;
        // 4. 当前的配方
        CurrentFormula = formula.DeepClone();
    }

    private void DownloadFormula()
    {
        try
        {

            foreach (var prop in typeof(FormulaEntity).GetProperties())
            {
                Debug.WriteLine(prop.Name);
                // 寻找对应的PLC地址块
                var address = _globalConfig.WriteEntityList
                    .FirstOrDefault(x => x.En == prop.Name)
                    ?.Address;

                if (address == null)
                {
                    continue;
                }

                // 判断PLC是否连接，连接则进行下一步
                if (!_globalConfig.PlcConnected)
                {
                    _userSession.ShowMessageBox("Plc未连接或连接异常");
                    return;
                }

                // 5. 下载数据
                var value = prop.GetValue(CurrentFormula);
                if (value == null)
                {
                    continue;
                }
                _globalConfig.Plc.WriteAsync(address, (float)value);
            }
            _userSession.ShowMessageBox("写入成功！");
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
        }

    }

    private void DeleteFormula()
    {
        if (SelectFormula  == null)
        {
            _userSession.ShowMessageBox("请选择要删除的配方");
            return;
        }

        if (MessageBox.Show("确定要删除配方吗？", "删除配方", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            FormulaList.Remove(SelectFormula);
            var res =  SqlSugarHelper.Db.Deleteable(SelectFormula).ExecuteCommand();
            if (res > 0)
            {
                _userSession.ShowMessageBox("删除成功");
            }
            else
            {
                _userSession.ShowMessageBox("删除失败");
            }
           
        }
       
    }

    private void SaveFormula()
    {
        try
        {
            // 1. 验证必填的字段 
            if (string.IsNullOrEmpty(CurrentFormula.Name))
            {
                _userSession.ShowMessageBox("配方名称不能为空！");
                return;
            }

            if (string.IsNullOrEmpty(CurrentFormula.Description))
            {
                _userSession.ShowMessageBox("配方描述不能为空！");
                return;
            }

            // 2. 如果编辑的是现有的配方，就更新配方
            if (SelectFormula != null)
            {
                var existFormula = FormulaList.FirstOrDefault(x => x.Id == SelectFormula.Id);
                if (existFormula != null)
                {
                    CurrentFormula.UpdateTime = DateTime.Now;
                    var index = FormulaList.IndexOf(existFormula);
                    FormulaList[index] = CurrentFormula;
                }
                else
                {
                    _userSession.ShowMessageBox("配方不存在");
                    return;
                }
            }
            else
            {
                // 3. 如果是新建的配方，就插入配方
                CurrentFormula.CreateTime = DateTime.Now;
                CurrentFormula.UpdateTime = DateTime.Now;
                FormulaList.Add(CurrentFormula);
            }

            // 4. 插入数据库
            var res = SqlSugarHelper.Db.Storageable<FormulaEntity>(FormulaList).ExecuteCommand();
            if (res > 0)
            {
                _userSession.ShowMessageBox("保存成功");
            }
            else
            {
                _userSession.ShowMessageBox("保存失败");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
        }
    }

    private void CreateFormula()
    {
        SelectFormula = null;
        CurrentFormula = new FormulaEntity();
    }

    private void QueryFormula()
    {
        FormulaList.Clear();
        SqlSugarHelper.Db.Queryable<FormulaEntity>().OrderBy(x => x.UpdateTime, SqlSugar.OrderByType.Desc)
            .ToList()
            .ForEach(x => FormulaList.Add(x));
    }
}