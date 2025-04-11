using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection; 
using System.Threading;
using System.Threading.Tasks;
using GlucacxeScadaSystem.Helpers; 
using GlucacxeScadaSystem.Models;  
using Microsoft.Extensions.Options;
using Prism.Mvvm;

namespace GlucacxeScadaSystem.ViewModels;


public class IndexViewModel : BindableBase, IDisposable
{
    private readonly GlobalConfig _globalConfig;
    private readonly RootParam _rootParam; 
    private CancellationTokenSource _cts = new(); 

    private ScadaReadData _scadaReadData = new();
    public ScadaReadData ScadaReadData
    {
        get => _scadaReadData;
        
        private set => SetProperty(ref _scadaReadData, value);
    }

   
    public IndexViewModel(GlobalConfig globalConfig, RootParam rootParam)
    {
        _globalConfig = globalConfig;
        _rootParam = rootParam; // 获取当前的配置值

        StartCollectionBasedOnConfig(); // 根据配置启动采集
        StartUiUpdateLoopAsync();       // 启动 UI 更新循环任务
    }

  
    private void StartCollectionBasedOnConfig()
    {
        _globalConfig.InitPlcServer();

       
        if (_rootParam.PlcParam?.AutoCollect == true) // 添加了空值检查以确保安全
        {
            _globalConfig.StartCollectionAsync();
        }
        else
        {
            Debug.WriteLine("配置中 AutoCollect 已禁用。PLC 采集未自动启动。");
           
        }
    }

   
    private void StartUiUpdateLoopAsync()
    {
        Task.Run(async () =>
        {
            var properties = typeof(ScadaReadData)
                .GetProperties()
                
                .Where(p => p.CanWrite && (p.PropertyType == typeof(float) || p.PropertyType == typeof(bool)))
                .ToList(); 

            while (!_cts.Token.IsCancellationRequested) 
            {
                try 
                {

                    foreach (var property in properties)
                    {
                        try 
                        {
                            if (property.PropertyType == typeof(float))
                            {
                                var value = _globalConfig.GetValue<float>(property.Name);
                                property.SetValue(ScadaReadData, value);
                            }
                            else if (property.PropertyType == typeof(bool))
                            {
                                var value = _globalConfig.GetValue<bool>(property.Name);
                                property.SetValue(ScadaReadData, value);
                            }
                        }
                        catch (KeyNotFoundException knfex) 
                        {
                            Debug.WriteLine($"警告: 在 GlobalConfig 中未找到键 '{property.Name}'。{knfex.Message}");
                        }
                        catch (Exception e) 
                        {
                            Debug.WriteLine($"处理属性 '{property.Name}' 时出错: {e}");
                        }
                    }
                    

                    await Task.Delay(100, _cts.Token);
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine("UI 更新循环已取消。");
                    break;
                }
                catch (Exception ex) 
                {
                    Debug.WriteLine($"UI 更新循环出错: {ex}");
                    
                    if (!_cts.Token.IsCancellationRequested)
                    {
                        try { await Task.Delay(500, _cts.Token); } catch {  }
                    }
                }
            }
        }, _cts.Token); 
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