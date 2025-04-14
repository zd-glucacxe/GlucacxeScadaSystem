using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastReport.MSChart;
using FastReport.Utils;
using GlucacxeScadaSystem.Helpers;
using GlucacxeScadaSystem.Models;
using HslCommunication;
using HslCommunication.Profinet.Siemens;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using SqlSugar;

namespace GlucacxeScadaSystem.ViewModels;

public class ParamsViewModel : BindableBase
{
    public RootParam RootParam { get; }

    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private SiemensS7Server _simulatedServer;

    private ScadaReadData _scadaReadData = new();
    public ScadaReadData ScadaReadData
    {
        get => _scadaReadData;
        private set => SetProperty(ref _scadaReadData, value);
    }

    private readonly GlobalConfig _globalConfig;
    private CancellationTokenSource _cts;

    public DelegateCommand ToggleCollectCommand { get; private set; }
    public DelegateCommand ToggleAutoMockCommand { get; private set; }
    public DelegateCommand TogglePlcServerCommand { get; private set; }


    public ParamsViewModel(RootParam rootParam, GlobalConfig globalConfig)
    {
        RootParam = rootParam;
        _globalConfig = globalConfig;

        ToggleCollectCommand = new DelegateCommand(ToggleCollection);
        ToggleAutoMockCommand = new DelegateCommand(ToggleAutoMock);
        TogglePlcServerCommand = new DelegateCommand(TogglePlcServer);
    }



    private void TogglePlcServer()
    {
        if (RootParam.PlcParam.AutoPlcServer)
        {
            StartPlcServer();
        }
        else
        {
            StopPlcServer();
        }
    }

    private async void StartPlcServer()
    {
        _simulatedServer = new SiemensS7Server();
        Authorization.SetAuthorizationCode("d1c914f5-2159-49f0-9c7e-2a6c5cd29e55");

        _logger.Info($"开始仿真西门子Plc-1200 at {DateTime.Now}");

         _simulatedServer.ServerStart(102);


        _logger.Info("仿真服务已调用启动指令，等待服务就绪...");
        await WaitForServerReadyAsync("127.0.0.1", 102);

        _logger.Info("仿真PLC服务已就绪。");

        // 启动服务 这里先启动硬件仿真 然后再启动plc服务
        await InitPlcServer();
    }

    private async Task InitPlcServer()
    {
        await _globalConfig.InitPlcServer();
    }

    private async Task<bool> WaitForServerReadyAsync(string ip, int port, int timeoutMs = 5000)
    {
        var startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
        {
            try
            {
                using var client = new System.Net.Sockets.TcpClient();
                await client.ConnectAsync(ip, port);
                return true;
            }
            catch
            {
                await Task.Delay(200);
            }
        }

        return false;
    }


    private void StopPlcServer()
    {
        // 退出时关闭服务
        _simulatedServer?.ServerClose();
        _simulatedServer = null;
        _logger.Info("已关闭仿真西门子Plc-1200");
    }


    private void ToggleAutoMock()
    {
        if (RootParam.PlcParam.AutoMock)
        {
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


    private void ToggleCollection()
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