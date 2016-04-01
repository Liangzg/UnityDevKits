/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 描述：基于LRU（最近最少使用算法）的缓存器
/// <para>创建时间：2016-3-18</para>
/// </summary>
public class LRUCache<K , V>
{
    /// <summary>
    /// 最大容量
    /// </summary>
    private int capacityMaxCount;
    
    /// <summary>
    /// 头结点
    /// </summary>
    private Node<K, V> header;
    /// <summary>
    /// 尾结点
    /// </summary>
    private Node<K, V> tail;  

    private Dictionary<K , Node<K , V>> cacheDic = new Dictionary<K, Node<K, V>>();

    public Action<Node<K, V>> DestroyAction; 
    public LRUCache() : this(10)
    {} 

    public LRUCache(int capacity)
    {
        this.capacityMaxCount = capacity;
        
        this.initizalie();   
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void initizalie()
    {
        header = new Node<K, V>();
        tail = new Node<K, V>();

        header.Next = tail;
        tail.Previous = header;
    }


    public Node<K, V> GetNode(K key)
    {
        if (!cacheDic.ContainsKey(key))
        {
//            Debug.LogWarning("<<LRUCache , GetNode>> Cant find !! Key is " + key);
            return null;
        }

        Node<K, V> node = cacheDic[key];
        detach(node);
        attach(node);
        return node;
    }


    public V GetValue(K key)
    {
        Node<K, V> node = GetNode(key);
        return node == null ? default(V) : node.Value;
    }

    /// <summary>
    /// 将数据添加到缓存链
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    public void Put(K key, V data)
    {
        Node<K, V> curNode = null;
        if (!cacheDic.ContainsKey(key))
        {
            curNode = new Node<K, V>(key , data);
            cacheDic[key] = curNode;
        }
        else
        {
            detach(cacheDic[key]);
        }
        curNode = cacheDic[key];

        //将结点插入到头部
        attach(curNode);

        //清理缓存链
        this.ClearCache();
    }

    /// <summary>
    /// 拆分结点
    /// </summary>
    /// <param name="node"></param>
    private void detach(Node<K, V> node)
    {
        node.Next.Previous = node.Previous;
        node.Previous.Next = node.Next;
    }

    /// <summary>
    /// 将结点插入头部
    /// </summary>
    /// <param name="node"></param>
    private void attach(Node<K, V> node)
    {
        node.Previous = header;
        node.Next = header.Next;

        header.Next = node;
        node.Next.Previous = node;
    }

    /// <summary>
    /// 缓存链大小
    /// </summary>
    public int Size {   get { return cacheDic.Count; } }

    public void ClearCache()
    {
        if (Size <= capacityMaxCount) return;

        Node<K, V> curTail = tail.Previous;
        cacheDic.Remove(curTail.Key);
        detach(curTail);

        if (DestroyAction != null) DestroyAction(curTail);

      //重复迭代
      ClearCache();
    }
    /// <summary>
    /// 删除结点
    /// </summary>
    /// <param name="key"></param>
    public void RemoveKey(K key)
    {
        if (!cacheDic.ContainsKey(key)) return;

        Node<K, V> node = cacheDic[key];
        detach(node);
        cacheDic.Remove(key);
    }

    /// <summary>
    /// 设置容器大小
    /// </summary>
    /// <param name="max"></param>
    /// <param name="update"></param>
    public int Capactity
    {
        get { return capacityMaxCount; }
        set
        {
            capacityMaxCount = value;
            ClearCache();
        }

    }




    /// <summary>
    /// 容器是否已满，如果已满则返回true，否则false
    /// </summary>
    public bool Full {  get { return cacheDic.Count >= capacityMaxCount; }}

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        Node<K, V> node = header.Next;
        while (node != null && node != tail)
        {
            sb.AppendFormat("{0}:{1}\r\n", node.Key, node.Value);
            node = node.Next;
        }
        return sb.ToString();
    }


}
