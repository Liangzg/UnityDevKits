/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// 描述：Act配置本地格式，以JSON的格式存储
/// <para>创建时间：</para>
/// </summary>
public class JsonLocalData : ILocalData {
    
    public bool Export<T>(string path, object[] objArr) where T : Attribute
    {
        if (objArr == null || objArr.Length <= 0 || string.IsNullOrEmpty(path))
        {
            return false;
        }

        if (!path.EndsWith(".json")) path += ".json";

        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        foreach (object obj in objArr)
        {
            sb.AppendFormat("{{\"type\":\"{0}\",", obj.GetType().Name);
            sb.Append("\"data\":{");
            Dictionary<string, object> reflectDic = InjectDataBinder.ReflectAllData<T>(obj);
            saveJson(sb , reflectDic);
            sb.Append("}},");
        }
        if (sb[sb.Length - 1] == ',')
            sb.Remove(sb.Length - 1, 1);

        sb.Append("]");

        FileGameUtil.WriteText(path ,sb.ToString());
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        return true;
    }

    /// <summary>
    /// 保存json数据
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="reflectDic"></param>
    private void saveJson(StringBuilder sb , Dictionary<string, object> reflectDic)
    {
        foreach (string key in reflectDic.Keys)
        {
            if (reflectDic[key] is Dictionary<string, object>)
            {
                sb.AppendFormat("\"{0}\":{{", key);
                saveJson(sb , reflectDic[key] as Dictionary<string , object>);
                sb.Append("},");
            }
            else
            {
                sb.AppendFormat("\"{0}\":\"{1}\"," , key , reflectDic[key]);
            }
        }

        //移除最末位的逗后
        if (sb[sb.Length - 1] == ',')
            sb.Remove(sb.Length - 1, 1); 
    }

    public object[] Import(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (!path.EndsWith(".json")) path += ".json";

        string txt = FileGameUtil.ReadText(path);
        if (string.IsNullOrEmpty(txt)) return null;
        
        List<object> instanceList = new List<object>();
        ArrayList dataJson = MiniJson.jsonDecode(txt) as ArrayList;
        foreach (object obj in dataJson)
        {
            Hashtable itemTab = obj as Hashtable;
            Type classType = Type.GetType(Convert.ToString(itemTab["type"]));

            Hashtable dataTab = itemTab["data"] as Hashtable;
            Dictionary<string , object> reflectDic = CopyTo(dataTab);
            object instance = InjectDataBinder.BindingData(classType, reflectDic);
            if(instance != null)    instanceList.Add(instance);
        }
        return instanceList.ToArray();
    }


    private Dictionary<string, object> CopyTo(Hashtable tb)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        foreach (object key in tb.Keys)
        {
            string keyStr = Convert.ToString(key);
            if (tb[key] is Hashtable)
            {
                Hashtable subTab = tb[key] as Hashtable;
                dic[keyStr] = CopyTo(subTab);
            }else
                dic[keyStr] = tb[key];
        }
        return dic;
    }

}
