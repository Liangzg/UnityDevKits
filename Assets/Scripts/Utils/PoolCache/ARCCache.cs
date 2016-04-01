/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using System.Text;

/// <summary>
/// 描述：Adaptive Replacement Cache
/// <para>创建时间：2016-3-23</para>
/// </summary>
public class ARCCache<K , V>
{

    private LRUCache<K, V> ruCache;
    private LRUCache<K, V> ruGhostCache;

    private LFUCache<K, V> fuCache;
    private LFUCache<K, V> fuGhostCache;

    private int initCapactity;

    public Action<Node<K, V>> DestroyCallback; 
    public ARCCache(int initCacheCount)
    {
        this.initCapactity = initCacheCount;
        ruCache = new LRUCache<K, V>(initCacheCount);
        ruCache.DestroyAction = addGhostLRUCache;
        ruGhostCache = new LRUCache<K, V>(initCacheCount);
        ruGhostCache.DestroyAction = DestroyCallback;  //释放数据并清空记录

        fuCache = new LFUCache<K, V>(initCacheCount);
        fuCache.DestroyAction = addGhostLFUCache;
        fuGhostCache = new LFUCache<K, V>(initCacheCount);
        fuGhostCache.DestroyAction = DestroyCallback; //释放数据并清空记录
    } 

    public V Get(K key)
    {
        //从LFU缓存中进行命中
        Node<K, V> node = fuCache.GetNode(key);
        if(node != null)    return node.Value;

        //从LRU缓存中进行命中
        node = ruCache.GetNode(key);
        if (node != null)
        {
            //lru缓存链中被命中，则添加到LFU中
            ruCache.RemoveKey(key);
            fuCache.Put(key, node.Value);
            return node.Value;
        }

        return default(V);
    }

    /// <summary>
    /// 添加元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Put(K key, V value)
    {
        //先从LRU缓存中进行命中测试
        Node<K, V> node = ruCache.GetNode(key);
        if (node != null)
        {
            //lru缓存链中被命中，则添加到LFU中
            ruCache.RemoveKey(key);
            fuCache.Put(key , value);
            return;
        }

        //从LFU中进行命中测试
        node = fuCache.GetNode(key);
        if (node != null) return;

        //检测幽灵命中，已经被释放的资源重新添加
        int maxCapactity = initCapactity + initCapactity / 2;
        node = ruGhostCache.GetNode(key);
        if (node != null)
        {
            if (ruCache.Full && ruCache.Capactity < maxCapactity)
            {
                ruCache.Capactity++;
                fuCache.CapacityMax--;
            }
            ruCache.Put(key , value);
            ruGhostCache.RemoveKey(key);
            return;
        }

        node = fuGhostCache.GetNode(key);
        if (node != null)
        {
            if (fuCache.Full && fuCache.CapacityMax < maxCapactity)
            {
                fuCache.CapacityMax++;
                ruCache.Capactity --;
            }

            LFUCache<K, V>.LFUNode<K, V> fuNode = node as LFUCache<K, V>.LFUNode<K, V>;
            fuCache.Put(key , value , fuNode.RefCount);
            fuGhostCache.RemoveKey(key);
            return;
        }

        //新资源处理
        ruCache.Put(key, value);
    }

    /// <summary>
    /// 当前缓存大小
    /// </summary>
    public int Size
    {
        get { return ruCache.Size + fuCache.Size; }
    }

    /// <summary>
    /// 添加到镜像LRU缓存
    /// </summary>
    /// <param name="node"></param>
    private void addGhostLRUCache(Node<K, V> node)
    {
        ruGhostCache.Put(node.Key , node.Value);
        if(DestroyCallback != null)
            DestroyCallback.Invoke(node);
    }

    /// <summary>
    /// 添加到镜像LFU缓存
    /// </summary>
    /// <param name="node"></param>
    private void addGhostLFUCache(Node<K, V> node)
    {
        LFUCache<K, V>.LFUNode<K, V> fuNode = node as LFUCache<K, V>.LFUNode<K, V>;
        fuGhostCache.Put(node.Key , node.Value , fuNode.RefCount);

        //只会销毁缓存数据，而会保留缓存引用记录
        if (DestroyCallback != null)
            DestroyCallback.Invoke(node);
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(ruCache.ToString());
        sb.AppendLine(fuCache.ToString());
        return sb.ToString();
    }
}
