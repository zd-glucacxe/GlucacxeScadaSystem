using System;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.VisualBasic;
using Prism.Mvvm;
using SqlSugar;

namespace GlucacxeScadaSystem.Models;

/// <summary>
/// 实体基类
/// </summary>
public class EntityBase : BindableBase
{
    // 主键 自增
    private int _id;
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private DateTime _createTime = DateTime.Now;
   
    public DateTime CreateTime
    {
        get => _createTime;
        set => SetProperty(ref _createTime, value);
    }


    private DateTime _updateTime = DateTime.Now;

    public DateTime UpdateTime
    {
        get => _updateTime;
        set => SetProperty(ref _updateTime, value);
    }


}