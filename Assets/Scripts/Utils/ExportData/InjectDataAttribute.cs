/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// 描述：数据注入
/// <para>创建时间：2016-03-12</para>
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class InjectDataAttribute : Attribute
{

    private string displayName;
    private bool isMust;
    public InjectDataAttribute() { }

    public InjectDataAttribute(object obj)
    {
        name = obj;
    }

    public InjectDataAttribute(string name)
    {
        this.displayName = name;
    }

    public InjectDataAttribute(string name, bool must)
    {
        this.displayName = name;
        this.isMust = must;
    }

    public object name { get; set; }

    /// <summary>
    /// 编辑器界面使用参数，表示界面中的文字显示
    /// </summary>
    public string DisplayName {  get {return displayName;} }

    /// <summary>
    /// 是否是必要的字段
    /// </summary>
    public bool IsMust { get { return isMust; } }
}
