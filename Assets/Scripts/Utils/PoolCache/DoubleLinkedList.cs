/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using System.Collections.Generic;

/// <summary>
/// 描述：两向链表
/// <para>创建时间：2016-3-21</para>
/// </summary>
public class DoubleLinkedList<K , V>
{


    private Node<K, V> mHead;

    private Node<K, V> mTail;

    private Dictionary<K , Node<K , V>> linkedLog = new Dictionary<K, Node<K, V>>(); 

    public DoubleLinkedList()
    {
        mHead = new Node<K, V>();
        mTail = new Node<K, V>();

        mHead.Next = mTail;
        mHead.Next.Previous = mHead;
    }

    /// <summary>
    /// 将结点添加到尾部
    /// </summary>
    /// <param name="node"></param>
    public void AddTail(Node<K, V> node)
    {
        //添加原链表包含此结点，则先将结点关系拆分
        if (linkedLog.ContainsKey(node.Key))
            detach(node);
        linkedLog[node.Key] = node;

        node.Next = mTail;
        node.Previous = mTail.Previous;

        mTail.Previous = node;
        node.Previous.Next = node;
    }

    /// <summary>
    /// 将结点添加到头部
    /// </summary>
    /// <param name="node"></param>
    public void AddHead(Node<K, V> node)
    {
        //添加原链表包含此结点，则先将结点关系拆分
        if (linkedLog.ContainsKey(node.Key))
            detach(node);
        linkedLog[node.Key] = node;

        node.Previous = mHead;
        node.Next = mHead.Next;

        mHead.Next = node;
        node.Next.Previous = node;
    }


    /// <summary>
    /// 拆分结点
    /// </summary>
    /// <param name="node"></param>
    private void detach(Node<K, V> node)
    {
        if(node == null)    return;
        node.Next.Previous = node.Previous;
        node.Previous.Next = node.Next;
    }

    /// <summary>
    /// 删除结点
    /// </summary>
    /// <param name="key"></param>
    public void RemoveKey(K key)
    {
        if (!linkedLog.ContainsKey(key)) return;

        Node<K, V> node = linkedLog[key];
        detach(node);
    }


    public void RemoveNode(Node<K, V> node)
    {
        if (node == null) return;
        RemoveKey(node.Key);
    }


    public Node<K, V> this[K key]
    {
        get
        {
            if (!linkedLog.ContainsKey(key)) return null;
            return linkedLog[key];
        }
    } 

    public int Size { get { return linkedLog.Count; } }


}
