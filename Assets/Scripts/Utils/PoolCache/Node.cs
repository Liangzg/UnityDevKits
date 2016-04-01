/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;

/// <summary>
/// 描述：
/// <para>创建时间：</para>
/// </summary>
public class Node<K, V>
{
    private K mKey;
    public V Value;

    public Node<K, V> Previous;
    public Node<K, V> Next;

    public Node() { }

    public Node(K key, V value)
    {
        mKey = key;
        Value = value;
    }

    public K Key
    {
        get { return mKey; }
    }
}
