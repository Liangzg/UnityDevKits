/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;

/// <summary>
/// 描述：编辑器特性常用接口
/// <para>创建时间：</para>
/// </summary>
public sealed class AttributeUtil  {

    /// <summary>
    /// 反射获得同一类型的
    /// </summary>
    /// <typeparam name="T">反射标记类型</typeparam>
    /// <param name="classType">指定程序集的类</param>
    /// <returns></returns>
    public static void GetAllInstanceClass<T>(List<object> list , Type classType)
    {
        Assembly editorAssembly = Assembly.GetAssembly(classType);

        Type[] typeArr = editorAssembly.GetTypes();
        foreach (Type type in typeArr)
        {
            object[] customTypeArr = type.GetCustomAttributes(typeof(T), false);

            if (customTypeArr.Length > 0)
            {
                list.Add(CreatInstance(type));
            }
        }
    }

    /// <summary>
    /// 深度遍历创建实例对象
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object CreatInstance(Type type)
    {
        object obj = Activator.CreateInstance(type);

        foreach (PropertyInfo propertyInfo in type.GetProperties())
        {
            Type propertyType = propertyInfo.PropertyType;
            if (!propertyType.IsValueType && propertyType != typeof (System.String) && !propertyType.IsEnum)
            {
                propertyInfo.SetValue(obj , CreatInstance(propertyType) , null);
            }
        }

        return obj;
    }

    /// <summary>
    /// 获得类字段指定标识列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<T> GetPropertyAttibutes<T>(object obj) where T : Attribute
    {
        List<T> list = new List<T>();
        FindAllAttribute(list , obj);
        return list;
    }

    /// <summary>
    /// 查找所有的指定Attributes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="obj"></param>
    public static void FindAllAttribute<T>(List<T> list, object obj) where T : Attribute
    {
        Type objType = obj.GetType();
        foreach (PropertyInfo propertyInfo in objType.GetProperties())
        {
            Attribute attribute = Attribute.GetCustomAttribute(propertyInfo, typeof(T));
            if (attribute == null) continue;

            Type proType = propertyInfo.GetType();
            if (proType.IsEnum || proType.IsValueType || proType.Equals(typeof (System.String)))
                list.Add(attribute as T);
            else
            {
                FindAllAttribute<T>(list , Activator.CreateInstance(proType));
            }
        }
    }
}
