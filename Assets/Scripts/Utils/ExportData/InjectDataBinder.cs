/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

/// <summary>
/// 描述：注入数据绑定器
/// <para>创建时间：</para>
/// </summary>
public class InjectDataBinder {

    /// <summary>
    /// 绑定相应对象的属性值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataDic"></param>
    /// <returns></returns>
    public static T BindingData<T>(Dictionary<string , object> dataDic) 
    {
        if (dataDic == null || dataDic.Count <= 0)
        {
            Debug.LogWarning("<<InjectDataBinder , BindingData>> data dictionary is null .");
            return default(T);
        }
        
        return (T)BindingData(typeof(T) , dataDic);
    }

    /// <summary>
    /// 循环遍历类的层次，进行数据绑定
    /// </summary>
    /// <param name="objType">object type</param>
    /// <param name="dataDic">类数据字典</param>
    /// <returns></returns>
    public static object BindingData(Type objType, Dictionary<string, object> dataDic)
    {
        object instance = Activator.CreateInstance(objType);
        foreach (PropertyInfo propertyInfo in objType.GetProperties())
        {
            Attribute injectAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(InjectDataAttribute), false);
            if (injectAttribute == null || !dataDic.ContainsKey(propertyInfo.Name)) continue;

            if (!propertyInfo.CanWrite) continue;

            Type propertyType = propertyInfo.PropertyType;
            if (propertyType.IsValueType || propertyType.IsEnum || propertyType.Equals(typeof(System.String)))
            {
                //绑定数据
                convertTo(instance ,propertyInfo , dataDic[propertyInfo.Name]);
            }
            else
            {
                Dictionary<string, object> subData = dataDic[propertyInfo.Name] as Dictionary<string, object>;

                object subObj = BindingData(propertyType , subData);
                if(subObj != null)  propertyInfo.SetValue(instance , subObj , null);
            }
        }
        return instance;
    }

    /// <summary>
    /// 转换数值
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="value"></param>
    private static void convertTo(object instance , PropertyInfo propertyInfo, object value)
    {
        Type propertyType = propertyInfo.PropertyType;
        if(propertyType == typeof(bool))
            propertyInfo.SetValue(instance, Convert.ToBoolean(value) , null);
        else if(propertyType == typeof(Int16))
            propertyInfo.SetValue(instance, Convert.ToInt16(value) , null);
        else if (propertyType == typeof(Int32))
            propertyInfo.SetValue(instance, Convert.ToInt32(value), null);
        else if (propertyType == typeof(Int64))
            propertyInfo.SetValue(instance, Convert.ToInt64(value), null);
        else if (propertyType == typeof (float))
        {
            propertyInfo.SetValue(instance, float.Parse(Convert.ToString(value)), null);
        }
        else if (propertyType == typeof (Vector3))
        {
            string text = Convert.ToString(value);
            string[] temp = text.Substring(1, text.Length - 2).Split(',');
            float x = float.Parse(temp[0]);
            float y = float.Parse(temp[1]);
            float z = float.Parse(temp[2]);
            Vector3 rValue = new Vector3(x, y, z);
            propertyInfo.SetValue(instance, rValue, null);
        }
        else if (propertyType == typeof(Vector2))
        {
            string text = Convert.ToString(value);
            string[] temp = text.Substring(1, text.Length - 2).Split(',');
            float x = float.Parse(temp[0]);
            float y = float.Parse(temp[1]);
            Vector2 rValue = new Vector2(x, y);
            propertyInfo.SetValue(instance, rValue, null);
        }
        else if (propertyType.IsEnum)
        {
            propertyInfo.SetValue(instance, Enum.Parse(propertyType, Convert.ToString(value)) , null);
        }
        else if (propertyType == typeof (Color))
        {
            string text = Convert.ToString(value);
            string[] temp = text.Substring(5, text.Length - 6).Split(',');
            float r = float.Parse(temp[0]);
            float g = float.Parse(temp[1]);
            float b = float.Parse(temp[2]);
            float a = float.Parse(temp[3]);

            Color _color = new Color(r , g , b , a);
            propertyInfo.SetValue(instance, _color, null);
        }
        else
        {
            propertyInfo.SetValue(instance, Convert.ToString(value), null);
        }
    }

    /// <summary>
    /// 反射类内容所有标注数据
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Dictionary<string, object> ReflectAllData<T>(object obj) where T : Attribute
    {
        if (obj == null)
        {
            Debug.LogWarning("<<InjectDataBind , ReflectAllData>> object is null !");
            return null;
        }

        Dictionary<string , object> dataDic = new Dictionary<string, object>();

        Type classType = obj.GetType();
        foreach (PropertyInfo propertyInfo in classType.GetProperties())
        {
            Attribute injectAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(T), false);
            if(injectAttribute == null) continue;

            Type propertyType = propertyInfo.PropertyType;
            if (propertyType.IsValueType || propertyType.IsEnum || propertyType.Equals(typeof (System.String)))
            {
                dataDic[propertyInfo.Name] = propertyInfo.GetValue(obj, null);
            }
            else
            {
                //添加包含子类对象
                Dictionary<string , object> valueDic = ReflectAllData<T>(propertyInfo.GetValue(obj, null));
                if (valueDic != null) dataDic[propertyInfo.Name] = valueDic;
            }
        }

        return dataDic;
    }
}
