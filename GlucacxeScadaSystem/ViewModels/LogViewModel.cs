using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GlucacxeScadaSystem.Models;
using NLog;
using Prism.Commands;
using Prism.Common;
using Prism.Mvvm;
using SqlSugar;

namespace GlucacxeScadaSystem.ViewModels;

public class LogViewModel : BindableBase
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private ObservableCollection<FileInfo> _logFiles = new();
    public ObservableCollection<FileInfo>  LogFiles
    {
        get => _logFiles;
        set => SetProperty(ref _logFiles, value);
    }

    private ObservableCollection<LogEntry> _logEntries = new();
    public ObservableCollection<LogEntry> LogEntries
    {
        get => _logEntries;
        set => SetProperty(ref _logEntries, value);
    }



    private FileInfo _selectedLogFile;
    public FileInfo SelectedLogFile
    {
        get => _selectedLogFile;
        set
        {
            // 使用 SetProperty 更新字段值，并引发 PropertyChanged 事件通知 UI
            // SetProperty 返回 true 表示值确实发生了改变
            if (SetProperty(ref _selectedLogFile, value))
            {
                OnSelectedLogFileChanged(value);
            }
        }
    }

    private void OnSelectedLogFileChanged(FileInfo value)
    {
        if (value != null)
        {
            try
            {
                var entries = new ObservableCollection<LogEntry>();
                var lines = File.ReadAllLines(value.FullName);  //文件路径
                foreach (var line in lines)
                {
                   var entry = LogEntry.Parse(line);
                    if (entry != null)
                    {
                        entries.Add(entry);
                    }
                }
                LogEntries = new ObservableCollection<LogEntry>(entries);

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }

 

    public DelegateCommand OpenLogFolderCommand { get; private set; }

    public LogViewModel()
    {
        OpenLogFolder();
        OpenLogFolderCommand = new DelegateCommand(OpenLogFolder);
    }

    private void OpenLogFolder()
    {
        try
        {
            LogFiles.Clear();

            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (Directory.Exists(logPath))
            {
                //遍历一周的日志文件
                var currDate = DateTime.Now;
                var startDate = currDate.AddDays(-30);
                var endDate = currDate;

                //从指定路径 logPath 下获取所有文件夹，筛选出文件夹名称是有效日期格式的，然后将这些文件夹转换为 DirectoryInfo 对象。
                var folders = Directory.GetDirectories(logPath)
                    .Where(dir => DateTime.TryParse(Path.GetFileName(dir), out _))
                    .Select(dir => new DirectoryInfo(dir));

                // 筛选出符合条件的文件夹
                var recentFolders = folders.Where(dir =>
                {
                    if (DateTime.TryParse(dir.Name, out DateTime foldDateTime)) 
                    {
                        return foldDateTime >= startDate && foldDateTime <= endDate;
                    }

                    return false;
                });

                // 获取满足日期的文件夹下的所有日志文件
                var logFiles = recentFolders.SelectMany
                (dir => dir.GetFiles("*.log", SearchOption.AllDirectories)
                    .OrderBy(f => f.LastWriteTime));

                LogFiles = new ObservableCollection<FileInfo>(logFiles);
            }

        }
       
        catch (Exception ex)
        {
            logger.Error(ex.Message);
        }
    }
}