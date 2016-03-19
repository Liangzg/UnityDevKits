/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// 描述：技能编辑器主窗口
/// <para>创建时间：2016-03-11</para>
/// </summary>
public class ActEditor : EditorWindow
{

//        private ILocalData localDataHelper = new XmlLocalData();
    private ILocalData localDataHelper = new JsonLocalData();
    /// <summary>
    /// Act组件模板列表
    /// </summary>
    private List<EditorWidgetState> widgetList = new List<EditorWidgetState>();
    
    /// <summary>
    /// 模块映射
    /// </summary>
    private Dictionary<ActModuleEnum, Type> moduleMap = new Dictionary<ActModuleEnum, Type>(); 
    /// <summary>
    /// 当前选择添加的新模块枚举
    /// </summary>
    private ActModuleEnum selectEnum;

    /// <summary>
    /// 属性面板内的位置
    /// </summary>
    private Vector2 propertyScrollPos;

    private Vector2 centerScrollPos;

    /// <summary>
    /// 右边的技能列表
    /// </summary>
    private EditorScrollWidgetState<object> rightActSkills;
    /// <summary>
    /// 左侧角色列表管理
    /// </summary>
    private ActRoleTree leftRoleTreeMgr;
    /// <summary>
    /// 当前的actGroup
    /// </summary>
    private string actGroup;
    /// <summary>
    /// 当前的Act名称
    /// </summary>
    private string actName;
    private void OnEnable()
    {
        List<object> classList = new List<object>();
        AttributeUtil.GetAllInstanceClass<ActModuleAttribute>(classList , typeof(AttributeUtil));
        AttributeUtil.GetAllInstanceClass<ActModuleAttribute>(classList , typeof(InjectDataAttribute));

        foreach (object obj in classList)
        {
            Type type = obj.GetType();
            object[] modea = type.GetCustomAttributes(typeof (ActModuleAttribute) , false);
            ActModuleAttribute modeAttribute = modea[0] as ActModuleAttribute;
            moduleMap[modeAttribute.ModuleEnum] = type;
        }

        //加载左边的角色列表
        leftRoleTreeMgr = new ActRoleTree(this);
        rightActSkills = new EditorScrollWidgetState<object>();
    }


    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        //left gui
        this.onLeftRoleTree();

        this.OnCenterGUI();

        //right skill list gui
        //this.onRightActListGUI(rightActSkills);

        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 左边的角色列表
    /// </summary>
    private void onLeftRoleTree()
    {
        GUILayout.BeginVertical(GUILayout.Width(250));

        leftRoleTreeMgr.OnGUI();

        
        GUILayout.EndVertical();
    }


    /// <summary>
    /// 中心区域的布局
    /// </summary>
    private void OnCenterGUI()
    {
        GUILayout.BeginVertical();

        centerScrollPos = EditorGUILayout.BeginScrollView(centerScrollPos);

        //绘制属性
        EditorGUIUtil.DrawHeader(actName + "属性");
        propertyScrollPos = EditorGUILayout.BeginScrollView(propertyScrollPos);

        List<EditorWidgetState> removes = null;
        foreach (EditorWidgetState obj in widgetList)
        {
            EditorGUI.indentLevel = 0;

            bool isRemove = EditorGUIUtil.OnGUIInstanceDisplay(obj, true);

            if (isRemove)
            {
                if (removes == null) removes = new List<EditorWidgetState>();
                removes.Add(obj);
            }
        }

        //remove
        if (removes != null && removes.Count > 0)
        {
            foreach (EditorWidgetState ews in removes)
                widgetList.Remove(ews);
        }
        EditorGUILayout.EndScrollView();


        EditorGUIUtil.DrawSeparator();
        //添加新模块视图
        this.onAddNewModuleGUI();

        if (!string.IsNullOrEmpty(actName))
        {
            EditorGUIUtil.DrawSeparator();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            //保存数据到本地
            if (GUILayout.Button("Save"))
            {
                this.saveActFile(actGroup , actName , true);
            }         
            GUILayout.Space(20);
            GUILayout.EndHorizontal();   
        }

        GUILayout.EndScrollView();
        GUILayout.Space(5F);
        GUILayout.EndVertical();
    }


    /// <summary>
    /// 保存ACT信息到本地文件
    /// </summary>
    /// <param name="groupName">分组名</param>
    /// <param name="actName">act名称</param>
    /// <param name="isTip">是否提示保存结果，true 提示</param>
    private void saveActFile(string groupName , string actName , bool isTip)
    {
        string relativePath = string.Concat("Act/", groupName, "/",actName, ".act");
        
        List<object> propertyList = new List<object>();
        foreach (EditorWidgetState widget in widgetList)
        {
            propertyList.Add(widget.Entity);
        }

        bool result = localDataHelper.Export<InjectDataAttribute>(relativePath, propertyList.ToArray());
        if (!isTip) return;

        if (!result)
            EditorUtility.DisplayDialog("错误" , "保存文件失败，详细情况，请查看日志" , "确定");
        else
            EditorUtility.DisplayDialog("提示", "已成功保存", "确定");
    }

    /// <summary>
    /// 右边的技能列表
    /// </summary>
    private void onRightActListGUI(EditorScrollWidgetState<object> scrollWidget)
    {
        GUILayout.BeginVertical();
        EditorGUIUtil.DrawHeader("技能列表");
        scrollWidget.Pos = GUILayout.BeginScrollView(scrollWidget.Pos);


        GUILayout.EndScrollView();
        GUILayout.EndVertical();        
    }

    /// <summary>
    /// 添加新的模块
    /// </summary>
    private void onAddNewModuleGUI()
    {
        EditorGUIUtil.DrawHeader("Add New Module");
        //添加模块视图
        GUILayout.BeginHorizontal("AS TextArea" , GUILayout.Height(30));
        selectEnum = (ActModuleEnum)EditorGUILayout.EnumPopup("新模块", selectEnum , GUILayout.MaxWidth(Screen.width * 0.5f));

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Add", GUILayout.Width(60)))
        {
            if (string.IsNullOrEmpty(actName)) return;

            EditorWidgetState widget = new EditorWidgetState();
            widget.Entity = Activator.CreateInstance(moduleMap[selectEnum]);
            widget.FoldOut = true;

            EditorGUIUtil.FindAllWidget(new List<EditorWidgetState>(), widget, widget.Entity);

            widgetList.Add(widget);
        }
        GUI.backgroundColor = Color.white;

        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 初始化ACT项的属性，从文件目录加载配置
    /// </summary>
    /// <param name="actItem"></param>
    public void InitActItemProperty(ActItemWidget actItem)
    {
        this.InitActItemProperty(actItem.GroupName ,actItem.ActName);
    }

    /// <summary>
    /// 初始化ACT项的属性，从文件目录加载配置
    /// </summary>
    /// <param name="groupName">Act 的分组</param>
    /// <param name="actName">Act 的名称</param>
    public void InitActItemProperty(string groupName, string actName)
    {
        this.actGroup = groupName;
        this.actName = actName;

        //加载文件
        string relativePath = string.Concat("Act/", groupName, "/", actName, ".act");

        object[] property = localDataHelper.Import(relativePath);
        widgetList.Clear();

        if (property == null) return;       //新建的文件没有本地数据
        int index = 0;
        foreach (object obj in property)
        {
            EditorWidgetState widget = new EditorWidgetState();
            widget.Entity = obj;
            widget.FoldOut = index == 0;

            EditorGUIUtil.FindAllWidget(new List<EditorWidgetState>(), widget, widget.Entity);

            index++;
            widgetList.Add(widget);
        }
    }

    /// <summary>
    /// 界面关闭时操作
    /// </summary>
    private void OnDestroy()
    {
        if(!string.IsNullOrEmpty(actName))
            this.saveActFile(actGroup , actName , false);
    }
}



#region ------------GUI折叠数据绑定----------------
/// <summary>
/// 编辑器组件状态
/// </summary>
public class EditorWidgetState
{
    /// <summary>
    /// 是否被折叠
    /// </summary>
    public bool FoldOut;

    /// <summary>
    /// 受影响的实体
    /// </summary>
    public object Entity;

    /// <summary>
    /// 是否可以被删除
    /// </summary>
    public bool CanRemove;

    /// <summary>
    /// 包含的子组件
    /// </summary>
    public EditorWidgetState[] SubWidget;
}


public class EditorScrollWidgetState<T>
{

    public Vector2 Pos;

    public T Entity;

}
#endregion