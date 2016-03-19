/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

/// <summary>
/// 描述：游戏项目File操作常用功能
/// <para>创建时间：</para>
/// </summary>
public sealed class FileGameUtil {

    /// <summary>
    ///  写入文本
    /// </summary>
    /// <param name="relativePath">相对路径</param>
    /// <param name="text">文本</param>
    public static void WriteText(string relativePath, string text)
    {
        if (string.IsNullOrEmpty(relativePath) || string.IsNullOrEmpty(text)) return;

        string allPath = string.Concat(ApplicationPath.PersistentRootPath, relativePath);
        byte[] byteArr = Encoding.UTF8.GetBytes(text);
        Write(allPath , byteArr);
    }

    /// <summary>
    /// 写文件
    /// </summary>
    /// <param name="path">绝对路径</param>
    /// <param name="fileBytes">文件的字节数组</param>
    public static void Write(string path, byte[] fileBytes)
    {
        ApplicationPath.CreateDirectory(path);
        FileStream stream = new FileStream(path, FileMode.Create);
        byte[] buf = new byte[4096];
        using (MemoryStream ms = new MemoryStream(fileBytes))
        {
            int count = 0;
            while ((count = ms.Read(buf, 0, buf.Length)) > 0)
            {
                stream.Write(buf, 0, count);
            }
            stream.Flush();
        }
        stream.Close();
    }



    public static byte[] Read(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Debug.LogWarning("<<FileGameUtil , Read>> Cant read file ! Path is " + path);
            return null;
        }

        FileStream fs = new FileStream(path , FileMode.Open);
        byte[] buf = new byte[4096];
        using (MemoryStream ms = new MemoryStream())
        {
            int count = 0;
            while ((count = fs.Read(buf , 0 , buf.Length)) > 0)
            {
                ms.Write(buf , 0 , count);
            }
            ms.Flush();
            buf = ms.ToArray();
        }
        fs.Close();
        return buf;
    }

    /// <summary>
    /// 读取文本
    /// </summary>
    /// <param name="relativePath">文本所在工程的相对路径</param>
    /// <returns></returns>
    public static string ReadText(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return null;

        string allPath = string.Concat(ApplicationPath.PersistentRootPath, relativePath);
        if (!File.Exists(allPath))
        {
            Debug.LogWarning("<<FileGameUtil , Read>> Cant read file ! Path is " + allPath);
            return null;
        }

        return File.ReadAllText(allPath);
    }
}
