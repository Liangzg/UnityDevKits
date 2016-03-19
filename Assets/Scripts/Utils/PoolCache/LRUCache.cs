/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
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


    public V Get(K key)
    {
        if (!cacheDic.ContainsKey(key))
        {
            Debug.LogWarning("<<LRUCache , Get>> Cant find !! Key is " + key);
            return default(V);
        }

        Node<K, V> node = cacheDic[key];
        detach(node);
        attach(node);
        return node.Value;
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
    }


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

    private class Node<K , V>
    {
        private K mKey;
        private V mVaule;

        public Node<K, V> Previous;
        public Node<K, V> Next; 
        
        public Node() { }  

        public Node(K key , V value)
        {
            mKey = key;
            mVaule = value;
        }

        public K Key
        {
            get { return mKey; }
        }


        public V Value { get { return mVaule; } }
    }
}
