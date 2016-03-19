/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 描述：编辑器GUI 常用功能
/// <para>创建时间：</para>
/// </summary>
public class EditorGUIUtil {

    /// <summary>
    /// Returns a blank usable 1x1 white texture.
    /// </summary>

    static public Texture2D blankTexture
    {
        get
        {
            return EditorGUIUtility.whiteTexture;
        }
    }

    /// <summary>
    /// 绘制实例
    /// </summary>
    /// <param name="widgetState"></param>
    /// <param name="isRemove"></param>
    /// <returns></returns>
    public static bool OnGUIInstanceDisplay(EditorWidgetState widgetState , bool isRemove)
    {
        object obj = widgetState.Entity;
        if (obj == null) return false;

        if (isRemove && widgetState.FoldOut) NGUIEditorTools.DrawSeparator();
        GUILayout.BeginHorizontal();
        
        widgetState.FoldOut = EditorGUILayout.Foldout(widgetState.FoldOut, widgetState.Entity.GetType().Name);
        
        if (isRemove && GUILayout.Button("X", GUILayout.Width(30)))
        {
            return true;
        }
        GUILayout.EndHorizontal();
        if (isRemove && widgetState.FoldOut)
        {
            NGUIEditorTools.DrawSeparator();
            GUILayout.Space(10);
        }

        //如果被折叠，则不绘制具体的面板信息
        if (!widgetState.FoldOut) return false;

        EditorGUI.indentLevel ++;

        Type objType = obj.GetType();
        foreach (PropertyInfo propertyInfo in objType.GetProperties())
        {
            Attribute attribute = Attribute.GetCustomAttribute(propertyInfo, typeof(InjectDataAttribute));
            if (attribute == null) continue;

            InjectDataAttribute display = attribute as InjectDataAttribute;
            if (string.IsNullOrEmpty(display.DisplayName)) continue;

            Type propertyType = propertyInfo.PropertyType;
            if (propertyType.IsEnum || propertyType.IsValueType || propertyType.Equals(typeof(System.String)))
            {

                GUILayout.BeginHorizontal();
                object result = OnGUIProperty(display.DisplayName, propertyType, propertyInfo.GetValue(obj, null));
                propertyInfo.SetValue(obj, result, null); //更新修改值
                GUILayout.EndHorizontal();
            }
        }

        //遍历绘制子层数据
        if (widgetState.SubWidget != null && widgetState.SubWidget.Length > 0)
        {
            EditorGUI.indentLevel++;
            foreach (EditorWidgetState subWidget in widgetState.SubWidget)
            {
                //bool lastFoldOut = subWidget.FoldOut;
                //NGUIEditorTools.DrawSeparator();
                
                //if(lastFoldOut)     NGUIEditorTools.DrawSeparator();

                OnGUIInstanceDisplay(subWidget , false);
            }
        }

        return false;
    }

    /// <summary>
    /// 绘制属性信息
    /// </summary>
    /// <param name="displayName">界面显示的名称</param>
    /// <param name="propertyType">属性数据</param>
    public static object OnGUIProperty(string displayName , Type propertyType, object property)
    {
        if (propertyType == typeof(bool))
        {
            property = EditorGUILayout.Toggle(displayName, (bool)property);
        }
        else if (propertyType == typeof(float))
        {
            property = EditorGUILayout.FloatField(displayName, (float)property);
        }
        else if (propertyType == typeof(int))
        {
            property = EditorGUILayout.IntField(displayName, (int)property);
        }
        else if (propertyType == typeof(double))
        {
            double v = (double)property;
            property = (double)EditorGUILayout.FloatField(displayName, (float)v);
        }
        else if (propertyType == typeof(Color))
        {
            property = EditorGUILayout.ColorField(displayName, (Color)property);
        }
        else if (propertyType == typeof(Vector2))
        {
            property = EditorGUILayout.Vector3Field(displayName, (Vector2)property);
        }
        else if (propertyType == typeof(Vector3))
        {
            property = EditorGUILayout.Vector3Field(displayName, (Vector3)property);
        }
        else
        {
            property = EditorGUILayout.TextField(displayName, property == null ? "" : property.ToString());
        }
        return property;
    }


    /// <summary>
    /// 查找所有的GUI可显示组件项
    /// </summary>
    /// <param name="widgets"></param>
    /// <param name="obj"></param>
    public static void FindAllWidget(List<EditorWidgetState> widgets ,EditorWidgetState parentWidget, object obj )
    {
        if (obj == null) return;

        Type objType = obj.GetType();
        foreach (PropertyInfo propertyInfo in objType.GetProperties())
        {
            Attribute attribute = Attribute.GetCustomAttribute(propertyInfo, typeof(InjectDataAttribute));
            if (attribute == null) continue;

            InjectDataAttribute display = attribute as InjectDataAttribute;
            if (string.IsNullOrEmpty(display.DisplayName)) continue;

            Type propertyType = propertyInfo.PropertyType;
            if (!propertyType.IsEnum && !propertyType.IsValueType && !propertyType.Equals(typeof(System.String)))
            {
                EditorWidgetState childWidget = new EditorWidgetState();
                object childObj = propertyInfo.GetValue(obj, null);
                childWidget.Entity = childObj == null ? Activator.CreateInstance(propertyType) : childObj;
                if(childObj == null)
                    propertyInfo.SetValue(obj , childWidget.Entity , null);
                widgets.Add(childWidget);

                FindAllWidget(new List<EditorWidgetState>(), childWidget, childWidget.Entity);
            }
        }
        parentWidget.SubWidget = widgets.ToArray();
    }


    /// <summary>
    /// Draw a visible separator in addition to adding some padding.
    /// </summary>

    static public void DrawSeparator()
    {
        GUILayout.Space(12f);

        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = blankTexture;
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.color = new Color(0f, 0f, 0f, 0.25f);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
            GUI.color = Color.white;
        }
    }



    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text) { return DrawHeader(text, text, false, NGUISettings.minimalisticLook); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false, NGUISettings.minimalisticLook); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, bool detailed) { return DrawHeader(text, text, detailed, !detailed); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (!minimalistic) GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUI.changed = false;

        if (minimalistic)
        {
            if (state) text = "\u25BC" + (char)0x200a + text;
            else text = "\u25BA" + (char)0x200a + text;

            GUILayout.BeginHorizontal();
            GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
        else
        {
            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
        }

        if (GUI.changed) EditorPrefs.SetBool(key, state);

        if (!minimalistic) GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }
}
