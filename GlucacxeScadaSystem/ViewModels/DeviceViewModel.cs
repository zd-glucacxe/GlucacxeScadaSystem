using System;
using System.Linq;
using GlucacxeScadaSystem.Helpers;
using GlucacxeScadaSystem.Models;
using GlucacxeScadaSystem.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace GlucacxeScadaSystem.ViewModels;

public class DeviceViewModel : BindableBase
{

    public readonly GlobalConfig GlobalConfigProp;

    private readonly UserSession _userSession;

    private LogNewLine _logNewLine = new();

    private string _logContent;
    public string LogContent
    {
        get => _logContent;
        set => SetProperty(ref _logContent, value);
    }

    private ScadaReadData _scadaReadData = new();
    public ScadaReadData ScadaReadData
    {
        get => _scadaReadData;
        private set => SetProperty(ref _scadaReadData, value);
    }


    public DelegateCommand<string> WriteDeviceControlCommand { get; private set; }

    public DelegateCommand ClearLogCommand { get; private set; }


    #region 工位开关命令
    public DelegateCommand<string> DegreasingStationCommand { get; private set; }
    public DelegateCommand<string> MoistureFurnaceCommand { get; private set; }
    public DelegateCommand<string> ThicknessStationCommand { get; private set; }
    public DelegateCommand<string> CoolingRoomCommand { get; private set; }
    public DelegateCommand<string> PhosphatingCommand { get; private set; }
    public DelegateCommand<string> CuringFurnaceCommand { get; private set; }
    public DelegateCommand<string> PrecisionCleaningCommand { get; private set; }
    public DelegateCommand<string> ConveyorCommand { get; private set; }

    #endregion

    public DeviceViewModel(GlobalConfig globalConfigProp, UserSession userSession)
    {
        GlobalConfigProp = globalConfigProp;
        _userSession = userSession;
        WriteDeviceControlCommand = new DelegateCommand<string>(WriteDeviceControl);
        ClearLogCommand = new DelegateCommand(ClearLog);

        DegreasingStationCommand = new DelegateCommand<string>(CommonStationExecute, CanExcute);
        MoistureFurnaceCommand = new DelegateCommand<string>(CommonStationExecute, CanExcute);
        ThicknessStationCommand = new DelegateCommand<string>(CommonStationExecute, CanExcute);
        CoolingRoomCommand = new DelegateCommand<string>(CommonStationExecute, CanExcute);
        PhosphatingCommand = new DelegateCommand<string>(CommonStationExecute, CanExcute);
        CuringFurnaceCommand = new DelegateCommand<string>(CommonStationExecute, CanExcute);
        PrecisionCleaningCommand = new DelegateCommand<string>(CommonStationExecute, CanExcute);
        ConveyorCommand = new DelegateCommand<string>(CommonStationExecute, CanExcute);

        Init();
    }

    private bool CanExcute(string arg)
    {
        return GlobalConfigProp.PlcConnected;
    }


    private void CommonStationExecute(string paramName)
    {
        if (!GlobalConfigProp.PlcConnected)
        {
            LogContent += _logNewLine.LogAdd("Plc未连接或连接异常！");
            _userSession.ShowMessageBox("Plc未连接或连接异常！");
            return;
        }

        var readEntry = GlobalConfigProp.ReadEntityList.FirstOrDefault(x => x.En == paramName);
        if (string.IsNullOrEmpty(readEntry?.Address))
        {
            LogContent += _logNewLine.LogAdd($"找不到{paramName}读地址");
            _userSession.ShowMessageBox($"找不到{paramName}读地址");
            return;
        }

        var value = (bool)ScadaReadData.GetType()
            .GetProperty(readEntry.En)?.GetValue(ScadaReadData);

        var res = GlobalConfigProp.Plc.Write(readEntry.Address, value);

        if (res.IsSuccess)
        {
            LogContent += _logNewLine.LogAdd($"写入{paramName} 地址{readEntry.Address} 写入值:{value}");
        }
    }




    private void ClearLog()
    {
        LogContent = "";
    }

    private void Init()
    {
        LogContent = _logNewLine.LogAdd("程序运行中...") + _logNewLine.LogAdd("程序已启动...");
    }


    /// <summary>
    /// 写入设备控制 做统一的处理，根据参数名称，找到对应的地址，然后写入PLC
    /// </summary>
    /// <param name="paramName">控制设备的相应名称</param>
    private void WriteDeviceControl(string paramName)
    {
        if (!GlobalConfigProp.PlcConnected)
        {
            LogContent += _logNewLine.LogAdd("Plc未连接或连接异常！");
            _userSession.ShowMessageBox("Plc未连接或连接异常！");
            return;
        }
        // 连接成功 做PLC下发
        var readAddress = GlobalConfigProp.ReadEntityList.FirstOrDefault(x => x.En == paramName)?.Address;
        if (string.IsNullOrEmpty(readAddress))
        {
            LogContent += _logNewLine.LogAdd($"找不到{paramName}读地址");
            _userSession.ShowMessageBox($"找不到{paramName}读地址");
            return;
        }

        // 拿到地址 写入Plc
        var res = GlobalConfigProp.Plc.Write(readAddress, true);

        if (res.IsSuccess)
        {
            // 记录日志
            LogContent += _logNewLine.LogAdd($"写入{paramName} 地址{readAddress} 写入值:True");
        }
    }
}