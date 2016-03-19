/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// 描述：Xml格式的本地数据
/// <para>创建时间：</para>
/// </summary>
public class XmlLocalData : ILocalData {

    public bool Export<T>(string path, object[] objArr)  where T : Attribute
    {
        if (objArr == null || objArr.Length <= 0 || string.IsNullOrEmpty(path))
        {
            return false;
        }

        if (!path.EndsWith(".xml")) path += ".xml";

        string allPath = string.Concat(ApplicationPath.PersistentRootPath, path);
        ApplicationPath.CreateDirectory(allPath);

        XmlDocument doc = new XmlDocument();
        XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
        XmlElement rootEle = doc.CreateElement("data");
        foreach (object obj in objArr)
        {
            Dictionary<string, object> dataDic = InjectDataBinder.ReflectAllData<T>(obj);
            
            XmlElement element = doc.CreateElement(obj.GetType().Name);
            this.saveXml(doc, element, dataDic);
            rootEle.AppendChild(element);
        }
        doc.AppendChild(dec);
        doc.AppendChild(rootEle);
        doc.Save(allPath);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        return true;
    }

    /// <summary>
    /// 深度遍历保存类属性
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="element"></param>
    /// <param name="dataDic"></param>
    private void saveXml(XmlDocument doc , XmlElement element, Dictionary<string, object> dataDic)
    {
        foreach (string key in dataDic.Keys)
        {
            if (dataDic[key] is Dictionary<string, object>)
            {
                Dictionary<string, object> subData = dataDic[key] as Dictionary<string , object>;
                XmlElement ele = doc.CreateElement(key);
                saveXml(doc , ele , subData);
                if (element != null) element.AppendChild(ele);
            }
            else
            {
                XmlAttribute attribute = doc.CreateAttribute(key);
                attribute.Value = dataDic[key].ToString();
                element.Attributes.Append(attribute);
            }
        }
    }

    public object[] Import(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        if (!path.EndsWith(".xml")) path += ".xml";

        string allPath = string.Concat(ApplicationPath.PersistentRootPath, path);
        if (!File.Exists(allPath))
        {
            UnityEngine.Debug.LogWarning("<<XmlLocalData , Import>> Cant find file . path is " + allPath);
            return null;
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(File.ReadAllText(allPath));

        XmlNode rootNode = doc.SelectSingleNode("data");

        List<object> deserialieList = new List<object>();
        Dictionary<string, object> dataDic = new Dictionary<string, object>();
        foreach (XmlNode childNode in rootNode.ChildNodes)
        {
            dataDic.Clear();
            readXml(childNode, dataDic);

            string nodeName = childNode.Name;
            Type classType = Type.GetType(nodeName);
            object objInstance = InjectDataBinder.BindingData(classType, dataDic);

            deserialieList.Add(objInstance);
        }
        return deserialieList.ToArray(); 
    }


    private void readXml(XmlNode node, Dictionary<string, object> dataDic)
    {
        foreach (object obj in node.Attributes)
        {
            XmlAttribute objAttribute = obj as XmlAttribute;
            dataDic.Add(objAttribute.Name , objAttribute.Value);
        }

        //遍历子结点
        if (node.HasChildNodes)
        {
            foreach (object childObj in node.ChildNodes)
            {
                XmlNode childNode = childObj as XmlNode;
                Dictionary<string , object> subDic = new Dictionary<string, object>();
                readXml(childNode, subDic);
                dataDic[childNode.Name] = subDic;
            }
        }
    }
}
