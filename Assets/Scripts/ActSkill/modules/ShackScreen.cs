/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.ComponentModel;

/// <summary>
/// 描述：
/// <para>创建时间：</para>
/// </summary>
[ActModule(ActModuleEnum.ShackScreen)]
public class ShackScreen
{

    [InjectData("震屏名称")]
    public string ShackName { get; set; }

    [InjectData("振震时间" , true)]
    public float Time { get; set; }

    [InjectData("其它参数")]
    public Vector3 Something { get; set; }

    [InjectData("颜色")]
    public Color color { get; set; }

    [InjectData("第二层数据")]
    public LocalDataParent DataParent{get;set;}
    public override string ToString()
    {
        return string.Format("Time: {0}, Something: {1}", Time, Something);
    }
}