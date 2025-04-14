using System;
using System.Diagnostics;

namespace GlucacxeScadaSystem.Models;

public class LogEntry : EntityBase
{

    /// <summary>
    /// 时间戳
    /// </summary>
    private DateTime _timeStamp;

    public DateTime TimeStamp
    {
        get => _timeStamp;
        set => SetProperty(ref _timeStamp, value);
    }


    /// <summary>
    /// 日志等级
    /// </summary>
    private string _level;

    public string Level
    {
        get => _level;
        set => SetProperty(ref _level, value);
    }


    /// <summary>
    /// 日志类别
    /// </summary>
    private string _logger;

    public string Logger
    {
        get => _logger;
        set => SetProperty(ref _logger, value);
    }


    /// <summary>
    /// 日志消息
    /// </summary>
    private string _message;

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }



    /// <summary>
    /// 日志内容解析
    /// </summary>
    /// <param name="logLine"></param>
    /// <returns></returns>
    public static LogEntry Parse(string logLine)
    {
        try
        {
            var parts = logLine.Split("|");

            if (parts.Length >= 4)
            {
                return new LogEntry()
                {
                    TimeStamp = DateTime.Parse(parts[0]),
                    Level = parts[1],
                    Logger = parts[2],
                    Message = parts[3]
                };
            }
            else
            {
                return new LogEntry()
                {
                    TimeStamp = DateTime.Now,
                    Level = "Error",
                    Logger = "ParseError",
                    Message = logLine
                };
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        return null;
    }
}

