using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastReport.MSChart;
using FastReport.Utils;
using GlucacxeScadaSystem.Helpers;
using GlucacxeScadaSystem.Models;
using Prism.Commands;
using Prism.Mvvm;
using SqlSugar;

namespace GlucacxeScadaSystem.ViewModels;

public class ParamsViewModel : BindableBase
{
    public RootParam RootParam { get; }

    private ScadaReadData _scadaReadData = new();
    public ScadaReadData ScadaReadData
    {
        get => _scadaReadData;
        private set => SetProperty(ref _scadaReadData, value);
    }

    private readonly GlobalConfig _globalConfig;
    private CancellationTokenSource _cts = new();

    public DelegateCommand  ToggleCollectCommand { get; private set; }

    public DelegateCommand ToggleAutoMockCommand { get; private set; }
    

    public ParamsViewModel(RootParam rootParam, GlobalConfig globalConfig)
    {
        RootParam = rootParam;
        _globalConfig = globalConfig;
        ToggleCollectCommand = new DelegateCommand(ToggleCollection);
        ToggleAutoMockCommand = new DelegateCommand(ToggleAutoMock);
    }
    private void ToggleAutoMock()
    {
        if (RootParam.PlcParam.AutoMock)
        {
            //StartAutoMockOld();
            StartAutoMock();
        }
        else
        {
            StopAutoMock();
        }
    }

    private void StopAutoMock()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }


    private void StartAutoMock()
    {
        _cts = new CancellationTokenSource();

        Task.Run(async () =>
        {

            // 1. 通过反射获取所有float类型、 Bool类型的属性 ScadaReadData
            var propertyFloat = typeof(ScadaReadData)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(float)).ToList();


            var propertyBool = typeof(ScadaReadData)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(bool)).ToList();

            while (!_cts.IsCancellationRequested)
            {
                // 2. 遍历每种属性，从一个gloableConfig 中读取对应的地址值，然后生成随机数写入到相应的地址中
                Random random = new Random();
                foreach (var property in propertyFloat)
                {
                    var value = GenerateRandomFloat(random);
                    var address = _globalConfig.ReadEntityList.FirstOrDefault(x => x.En == property.Name)?.Address;

                    if (!string.IsNullOrEmpty(address))
                    {
                        await _globalConfig.Plc.WriteAsync(address, value); // 写入随机数
                    }

                }
              
                foreach (var property in propertyBool)
                {
                    var value = GenerateRandomBool(random);
                    var address = _globalConfig.ReadEntityList.FirstOrDefault(x => x.En == property.Name)?.Address;
                    if (!string.IsNullOrEmpty(address))
                    {
                        await _globalConfig.Plc.WriteAsync(address, value); // 写入随机数
                    }
                }

                await Task.Delay(1500, _cts.Token);

            }
        }, _cts.Token);


    }


    private  void ToggleCollection()
    {
        if (RootParam.PlcParam.AutoCollect) // 表示配置文件为开启采集
        {
            _globalConfig.StopCollection();
            
            _globalConfig.StartCollectionAsync(); // 启动采集
         
        }
        else
        {
            _globalConfig.StopCollection();// 停止采集

        }
    }


    float GenerateRandomFloat(Random random)
    {
        // 生成 0.0 到 100.0 之间的随机浮点数
        return (float)(random.NextDouble() * 100.0);
    }

    bool GenerateRandomBool(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);
        return random.NextDouble() < 0.5;
    }


}