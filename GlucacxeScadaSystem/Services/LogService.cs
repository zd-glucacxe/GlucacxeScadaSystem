using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;

namespace GlucacxeScadaSystem.Services;

public static class LogService
{
    private static bool _isConfigured = false;

    public static Logger Logger { get; private set; }

    public static void AddLog(IConfiguration configuration)
    {
        if (_isConfigured) return;

        var logDir = Path.Combine(Environment.CurrentDirectory, "Logs");
        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }

        try
        {
            // 使用 appsettings.json 中的 "NLog" 配置节
            LogManager.Configuration = new NLogLoggingConfiguration(configuration.GetSection("NLog"));

            Logger = LogManager.GetCurrentClassLogger();
            _isConfigured = true;

            Logger?.Info("NLog 日志系统初始化成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"NLog 配置加载失败: {ex}");
        }
    }

  
}