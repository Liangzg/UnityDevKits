/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 描述：本地数据接口
/// <para>创建时间：</para>
/// </summary>
public interface ILocalData
{
    /// <summary>
    /// 导出数据
    /// </summary>
    /// <param name="path">导出的绝对路径</param>
    /// <param name="obj">需要导出的对象</param>
    /// <returns>导出数据的结果，导出成功返回true , 否则返回false</returns>
    bool Export<T>(string path , object[] obj) where T : Attribute;

    /// <summary>
    /// 导入数据，并返回对应的序列化对象
    /// </summary>
    /// <param name="path">数据存储的本地路径</param>
    /// <returns></returns>
    object[] Import(string path);

}




