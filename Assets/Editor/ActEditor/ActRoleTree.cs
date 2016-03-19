/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

/// <summary>
/// 描述：技能编辑器角色信息面板
/// <para>创建时间：</para>
/// </summary>
public class ActRoleTree
{

    private ActEditor mainEditor;
    /// <summary>
    /// 左边的角色列表
    /// </summary>
    private EditorScrollWidgetState<object> leftRoleTree;

    private Dictionary<string , EditorWidgetState> skillGroups;

    private string defaultGroupName = "New Group";
    private int indexGroup;
    private int lastIndexGroup;
    private string newActName = "New Act";

    private ActItemWidget selectActItem;
    private enum ActSkillTypeEnum
    {
        Attrack , Death , Hit  , Custom
    }
    private ActSkillTypeEnum actSkillType = ActSkillTypeEnum.Attrack;


    public ActRoleTree(ActEditor actEditor)
    {
        mainEditor = actEditor;
        this.initRoleLayout();
    }

    /// <summary>
    /// 初始化布局
    /// </summary>
    private void initRoleLayout()
    {
        leftRoleTree = new EditorScrollWidgetState<object>();
        skillGroups = new Dictionary<string, EditorWidgetState>();

        string dir = string.Concat(ApplicationPath.PersistentRootPath, "Act");
        if (!Directory.Exists(dir)) return;

        string[] dirAndFiles = Directory.GetFiles(dir, "*.act.*", SearchOption.AllDirectories);
        if (dirAndFiles == null) return;

        int index = 0;
        foreach (string file in dirAndFiles)
        {
            if (!Path.HasExtension(file) || file.EndsWith(".meta"))   continue;

            string groupName = Path.GetDirectoryName(file);
            groupName = groupName.Replace(dir, "").Replace("\\" , "");

            this.addSkillGroup(groupName);

            string fileName = Path.GetFileNameWithoutExtension(file);
            fileName = fileName.Replace(".act", "");
            this.addSkillAct(groupName , fileName);

            //设置默认选择项
            if(index == 0)  mainEditor.InitActItemProperty(groupName , fileName);
            index++;
        }
    }

    public void OnGUI()
    {
        EditorGUIUtil.DrawHeader("Act列表");
        leftRoleTree.Pos = GUILayout.BeginScrollView(leftRoleTree.Pos , false , true);

        foreach (EditorWidgetState group in skillGroups.Values)
        {
            ActSkillGroup skillGroup = group.Entity as ActSkillGroup;
            if(skillGroup.ActList == null || skillGroup.ActList.Count <= 0) continue;

            EditorGUI.indentLevel = 0;
            //            if (group.CanRemove) EditorGUILayout.BeginHorizontal();

            group.FoldOut = EditorGUILayout.Foldout(group.FoldOut, skillGroup.GroupName);

//            if (group.CanRemove)
//            {
//                if (GUILayout.Button("X", GUILayout.Width(25)))
//                {
//                    
//                }
//                EditorGUILayout.EndHorizontal();
//            }

            //绘制子列表
            if (skillGroup.ActList == null || skillGroup.ActList.Count <= 0 || !group.FoldOut) break;

            EditorGUI.indentLevel ++;
            List<object> removeList = null;  //删除列表
            foreach (ActItemWidget itemWidget in skillGroup.ActList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                
                if (itemWidget.IsSelect)
                    GUI.backgroundColor = Color.green;

                //选择编辑
                if (GUILayout.Button(itemWidget.ActName, GUILayout.MaxWidth(200)))
                {
                    if (selectActItem != null)
                        selectActItem.IsSelect = false;
                    itemWidget.IsSelect = true;
                    selectActItem = itemWidget;

                    //更新属性编辑区
                    mainEditor.InitActItemProperty(itemWidget);
                }
                GUI.backgroundColor = Color.white;

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    if(removeList == null)  removeList = new List<object>();
                    removeList.Add(itemWidget);
                }
                GUILayout.EndHorizontal();
            }

            //更新绘制列表
            if (removeList != null)
            {
                foreach (ActItemWidget obj in removeList)  skillGroup.ActList.Remove(obj);
            }
        }
        GUILayout.EndScrollView();


        GUILayout.BeginVertical("AS TextArea" , GUILayout.ExpandHeight(false) , GUILayout.MaxHeight(100));
        EditorGUIUtil.DrawHeader("Editor");
        //底部操作,添加分组
        GUILayout.BeginHorizontal();
        GUILayout.Label("Act组" , GUILayout.Width(60));

        string[] groupNameArr = new string[skillGroups.Count];
        skillGroups.Keys.CopyTo(groupNameArr, 0);

        if (groupNameArr.Length > 0 && (lastIndexGroup != indexGroup))
        {
            defaultGroupName = groupNameArr[indexGroup];
            lastIndexGroup = indexGroup;
        }

        defaultGroupName = GUILayout.TextField(defaultGroupName, GUILayout.MaxWidth(100));

        if (groupNameArr.Length > 0)
        {
            indexGroup = EditorGUILayout.Popup(indexGroup, groupNameArr, GUILayout.MaxWidth(100));
            GUILayout.Space(5);
        }
        GUILayout.EndHorizontal();

        //添加新的Act
        GUILayout.BeginHorizontal();
        GUILayout.Label("添加新ACT" , GUILayout.Width(60));
        actSkillType = (ActSkillTypeEnum)EditorGUILayout.EnumPopup(actSkillType, GUILayout.MaxWidth(100));

        GUILayout.EndHorizontal();

        if (actSkillType == ActSkillTypeEnum.Custom)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(70);
            newActName = GUILayout.TextArea(newActName, GUILayout.MaxWidth(100));
            GUILayout.EndHorizontal();
        }

        //add button
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Add"))
        {
            this.addSkillGroup(defaultGroupName);
            this.addSkillAct(defaultGroupName, newActName);
            newActName = "New Act";
        }
        GUI.backgroundColor = Color.white;
        GUILayout.Space(10);
        GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();

        GUILayout.Space(5f);
    }


    /// <summary>
    /// 添加角色组
    /// </summary>
    private void addSkillGroup(string name)
    {
        if (skillGroups.ContainsKey(name)) return;

        EditorWidgetState group = new EditorWidgetState();
        group.CanRemove = true;

        ActSkillGroup asg = new ActSkillGroup();
        asg.GroupName = name;

        group.Entity = asg;
        this.skillGroups.Add(name , group);

       // indexGroup = skillGroups.Count - 1;
    }

    /// <summary>
    /// 添加Act单元
    /// </summary>
    /// <param name="actName"></param>
    private void addSkillAct(string group , string actName)
    {
        EditorWidgetState groupWidget = skillGroups[group];

        ActSkillGroup skillGroup = groupWidget.Entity as ActSkillGroup;
        if(skillGroup.ActList == null)  
            skillGroup.ActList = new List<ActItemWidget>();

        //不允许相同组下，ACT重复
        foreach (ActItemWidget item in skillGroup.ActList)
        {
            if (item.ActName == actName) return;
        }

        ActItemWidget itemWidget = new ActItemWidget();
        itemWidget.GroupName = group;
        itemWidget.ActName = actName;
        skillGroup.ActList.Add(itemWidget);
    }


    private class ActSkillGroup
    {
        public string GroupName;

        public List<ActItemWidget> ActList;
    }



}


public class ActItemWidget
{
    //分组名称
    public string GroupName;

    public String ActName;
    /// <summary>
    /// 是否被选择
    /// </summary>
    public bool IsSelect;
}