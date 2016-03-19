/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// 描述：
/// <para>创建时间：</para>
/// </summary>
public class ApplicationPath {

    /// <summary>
    /// 各平台的持久化根目录
    /// </summary>
    public static string PersistentRootPath
    {
        get
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            return Application.dataPath + "/DevAssets/";
#elif UNITY_ANDROID || UNITY_IPHONE
           return Application.persistentDataPath;
#endif
            return Application.dataPath + "/DevAssets/";
        }
    }

    /// <summary>
    /// 创建目录
    /// </summary>
    /// <param name="path"></param>
    public static void CreateDirectory(string path)
    {
        string dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) &&!Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }


}
