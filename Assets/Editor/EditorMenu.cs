/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// 描述：编辑器扩展目录
/// <para>创建时间：</para>
/// </summary>
public sealed class EditorMenu {


    [MenuItem("Game/Act Editor")]
    private static void onActEditorWindows()
    {
        EditorWindow.GetWindow<ActEditor>("技能编辑器");
    }

}
