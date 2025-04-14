using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FastReport.Utils;
using GlucacxeScadaSystem.Models;
using HslCommunication;
using HslCommunication.Profinet.Siemens;
using ImTools;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using MiniExcelLibs;
using NLog;

namespace GlucacxeScadaSystem.Helpers;

public class GlobalConfig : IDisposable
{

    public SiemensS7Net Plc { get; set; }

    public bool PlcConnected { get; set; }

    /// <summary>                                          
    /// 数据字典，所有的读取数据都从这个字典里边读取 
    /// </summary>
    public ConcurrentDictionary<string, object> ReadDataDic { get; set; } = new();

    /// <summary>
    /// 从Excel读取的读地址列表, 相当于把Excel的每条数据变成List的元素
    /// </summary>
    public List<ReadEntity> ReadEntityList { get; set; } = new();

    /// <summary>
    /// 从Excel读取的写地址列表
    /// </summary>
    public List<WriteEntity> WriteEntityList { get; set; } = new();

    public readonly RootParam RootParam;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private CancellationTokenSource _cts = new();

    public GlobalConfig(RootParam rootParam)
    {
        RootParam = rootParam;
        InitPlc();
        InitExcelAddress();
    }

    /// <summary>
    /// 加载Excel数据 并写入相应的List
    /// </summary>
    private void InitExcelAddress()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "Configs");
        var readPath = Path.Combine(path, "TulingRead.xlsx");
        var writePath = Path.Combine(path, "TulingWrite.xlsx");

        if (!File.Exists(readPath) || !File.Exists(writePath))
        {
            _logger.Error($"找不到读/写的配置文件路径 xlsx{readPath}\n{writePath}");
            return;
        }

        try
        {
            ReadEntityList = MiniExcel.Query<ReadEntity>(readPath)
                .Where(x => !string.IsNullOrEmpty(x.Address))
                .ToList();

            WriteEntityList = MiniExcel.Query<WriteEntity>(writePath)
                .Where(x => !string.IsNullOrEmpty(x.Address))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error($"MiniExcel读取文件异常{ex.Message}");
        }
    }


    /// <summary>
    /// 初始化plc服务连接
    /// </summary>
    public async Task InitPlcServer()
    {
        try
        {
            await Task.Delay(1000); // 确保服务端已完全启动
            var res = await Plc.ConnectServerAsync();
            PlcConnected = res.IsSuccess;
            if (!res.IsSuccess)
            {
                _logger.Error($"PLC连接失败 {RootParam.PlcParam.PlcIp}:{RootParam.PlcParam.PlcPort}");
                MessageBox.Show($"PLC连接失败 {RootParam.PlcParam.PlcIp}:{RootParam.PlcParam.PlcPort}");
            }
        }
        catch (Exception ex)
        {
            PlcConnected = false;
            _logger.Error($"PLC连接异常：{ex.Message}");
        }
    }

    /// <summary>
    /// 开始采集写入读取字典 ReadDataDic
    /// </summary>
    /// <param name="token">任务取消令牌</param>
    /// <returns></returns>
    public Task StartCollectionAsync(CancellationToken? token = null)
    {
        StopCollection();

        //令牌创建
        _cts = CancellationTokenSource.CreateLinkedTokenSource(token ?? CancellationToken.None);

        return Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    // 批量读取赋值给字典
                    await UpdateControlData();  // 读取 DBX 的控制数据
                    await UpdateMonitorData();  // 读取 DBX 的监控数据
                    await UpdateProcessData();  // 读取 DBD 的过程数据（如 float）
                    await Task.Delay(RootParam.PlcParam.PlcCycleInterval, _cts.Token);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }
            }
        });
    }


    private async Task UpdateProcessData()
    {
        // 对于Model 为Data类型的 使用浮点读取...
        await UpdatePlcToReadDataDic<float>("Data", "DBD");
    }

    private async Task UpdateMonitorData()
    {
        await UpdatePlcToReadDataDic<bool>("Monitor", "DBX");
    }

    private async Task UpdateControlData()
    {
        await UpdatePlcToReadDataDic<bool>("Control", "DBX");
    }

    /// <summary>
    /// 读取数据到字典
    /// </summary>
    /// <typeparam name="T"> 读取的数据类型（如 bool 或 float）</typeparam>
    /// <param name="model">PLC 地址的模块名， 用于从 ReadEntityList 中筛选 Module == "xxx" 的项</param>
    /// <param name="addressType">地址类型</param>
    private async Task UpdatePlcToReadDataDic<T>(string model, string addressType)
    {
        try
        {
            // 从ReadEntityList 中筛选出符合指定model 和 addressType 的地址对象
            var address = ReadEntityList.Where(x => x.Module == model
                                                    && x.Address.Contains(addressType)).ToList();
            if (address.Count < 1)
            {
                return;
            }


            // PLC数据读取
            OperateResult<T[]> res;

            if (typeof(T) == typeof(bool))
            {
                // 从第一个地址开始，连续读取 N 个变量
                //address.First().Address 取该模块中第一个地址作为起始地址
                //(ushort)address.Count) 要读取的数据数量
                res = await Plc.ReadBoolAsync(address.First().Address, (ushort)address.Count) as OperateResult<T[]>;
                //Debug.WriteLine(address.First().Address);
                //Debug.WriteLine((ushort)address.Count);
            }
            else if (typeof(T) == typeof(float))
            {
                res = await Plc.ReadFloatAsync(address.First().Address, (ushort)address.Count) as OperateResult<T[]>;
            }
            else
            {
                _logger.Error("不支持传入的类型");
                return;
            }


            if (!res.IsSuccess)
            {
                _logger.Error("数据读取失败");
                return;
            }

            // 将Result结果放到字典中
            for (var i = 0; i < address.Count; i++)
            {
                if (ReadDataDic.ContainsKey(address[i].En))
                {
                    //Debug.WriteLine(address[i].ToString());
                    //Debug.WriteLine(address[i].En);
                    //Debug.WriteLine(res.Content[i]);
                    ReadDataDic[address[i].En] = res.Content[i];

                }
                else
                {
                    ReadDataDic.TryAdd(address[i].En, res.Content[i]);

                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }


    /// <summary>  
    /// 停止所有的异步任务
    /// </summary>
    public void StopCollection()
    {
        try
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }



    /// <summary>
    /// 初始化Plc，并设置参数 但不连接
    /// </summary>
    private void InitPlc()
    {
        Plc = new SiemensS7Net(SiemensPLCS.S1200, RootParam.PlcParam.PlcIp);
        Plc.Port = RootParam.PlcParam.PlcPort;
        Plc.Slot = RootParam.PlcParam.PlcSlot;
        Plc.Rack = RootParam.PlcParam.PlcRack;
        Plc.ConnectTimeOut = RootParam.PlcParam.PlcConnectTimeOut;
    }


    /// <summary>
    /// 获取实时字典数值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetValue<T>(string key)
    {
        if (ReadDataDic.TryGetValue(key, out object value))
        {
            return (T)value;
        }
        return default;
    }


    public void Dispose()
    {
        Dispose(true); // 调用受保护的 Dispose 方法，并指示正在进行显式释放
        GC.SuppressFinalize(this); // 通知垃圾收集器不需要再调用终结器
    }


    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 释放托管资源
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

        }

    }
}