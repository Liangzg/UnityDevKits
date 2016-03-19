/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// 描述：Act技能系统模块标识
/// <para>创建时间：</para>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ActModuleAttribute : Attribute
{

    private string modeName;


    public ActModuleAttribute(ActModuleEnum name)
    {
        this.modeName = name.ToString();
    }

    /// <summary>
    /// 模块名称
    /// </summary>
    public string ModeName
    {
        get { return modeName;}
    }

    public ActModuleEnum ModuleEnum
    {
        get { return (ActModuleEnum)Enum.Parse(typeof (ActModuleEnum), modeName); }
    }

}
