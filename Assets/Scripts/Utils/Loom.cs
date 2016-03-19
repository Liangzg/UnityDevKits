/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// 描述：自定义线和Unity主线程交互
/// <para>创建时间：</para>
/// </summary>
public class Loom : MonoBehaviour
{

    private static Loom mInstance;
    /// <summary>
    /// 最大线程数量
    /// </summary>
    public static int MAX_THRED_COUNT = 8;
    /// <summary>
    /// 当前的线和数量
    /// </summary>
    private static int curThreadNum;

    /// <summary>
    /// 用来支持延迟处理的列表
    /// </summary>
    private List<DelayedQueueItem> delayItems = new List<DelayedQueueItem>(); 

    private List<DelayedQueueItem> addActionList = new List<DelayedQueueItem>();
    private List<DelayedQueueItem> curActionList = new List<DelayedQueueItem>() ;  
    private static byte[] lockObj = new byte[0];
    public static Loom Instance
    {
        get
        {
            Initialize();
            return mInstance;
        }
    }


    void Awake()
    {
        mInstance = this;
        initialized = true;
    }

    static bool initialized;

    static void Initialize()
    {
        if (!initialized)
        {

            if (!Application.isPlaying)
                return;
            initialized = true;
            GameObject g = new GameObject("_Loom");
            g.hideFlags = HideFlags.HideInHierarchy;
            DontDestroyOnLoad(g);
            mInstance = g.AddComponent<Loom>();
        }

    }

    private float curTime;
    // Update is called once per frame
    void Update ()
	{
        curTime = Time.time;
        if (addActionList.Count > 0 || delayItems.Count > 0)
	    {
            lock (lockObj)
            {
                if (addActionList.Count > 0)
                {
                    curActionList.AddRange(addActionList);
                    addActionList.Clear();
                }
                

                if (delayItems.Count > 0)
                {
                    foreach (DelayedQueueItem delayItem in delayItems)
                    {
                        delayItem.Time += curTime;
                    }
                    curActionList.AddRange(delayItems);
                    delayItems.Clear();
                }
            }
        }


	    if (curActionList.Count > 0)
	    {
	        for (int i = curActionList.Count - 1; i >= 0; i--)
	        {
	            DelayedQueueItem item = curActionList[i];
                if(item == null || item.Callback == null ||  curTime < item.Time)  continue;
                item.Callback.Invoke();
                curActionList.RemoveAt(i);
	        }
	    }
	}

    /// <summary>
    /// 启动一个异步线程
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static Thread RunAsync(Action callback)
    {
        Initialize();
        while (curThreadNum >= MAX_THRED_COUNT)
        {
            Thread.Sleep(1);
        }

        Interlocked.Increment(ref curThreadNum);
        ThreadPool.QueueUserWorkItem(runAction, callback);
        return null;
    }


    private static void runAction(object action)
    {
        Action callbackAction = action as Action;
        if(callbackAction != null)
            callbackAction.Invoke();

        Interlocked.Decrement(ref curThreadNum);
    }

    /// <summary>
    /// 把执行内容加入到Unity主线和执行
    /// </summary>
    /// <param name="action"></param>
    /// <param name="time"></param>
    public static void QueueOnMainThread(Action action, float time)
    {
        DelayedQueueItem item = new DelayedQueueItem();
        item.Callback = action;
        item.Time = time;
        lock (lockObj)
        {
            if (time > 0)
            {
                Instance.delayItems.Add(item);
            }else 
            {
                Instance.addActionList.Add(item);
            }
        }
                
    }


    public static void QueueOnMainThread(Action action)
    {
        QueueOnMainThread(action , 0);
    }

    /// <summary>
    /// 延迟
    /// </summary>
    public class DelayedQueueItem
    {
        public float Time;
        public Action Callback;
    }
}
