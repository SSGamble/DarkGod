/****************************************************
	文件：TimerSvc.cs
	作者：CaptainYun
	日期：2019/05/30 21:56   	
	功能：定时服务
*****************************************************/

using System;
using System.Collections.Generic;

public class TimerSvc {
    /// <summary>
    /// 任务包
    /// </summary>
    class TaskPack {
        public int tid;
        public Action<int> cb;
        public TaskPack(int tid, Action<int> cb) {
            this.tid = tid;
            this.cb = cb;
        }
    }

    private static TimerSvc instance = null;
    public static TimerSvc Instance {
        get {
            if (instance == null) {
                instance = new TimerSvc();
            }
            return instance;
        }
    }
    PETimer pt = null;
    Queue<TaskPack> tpQue = new Queue<TaskPack>(); // 任务队列
    private static readonly string tpQueLock = "tpQueLock"; // 锁

    public void Init() {
        pt = new PETimer(100);
        tpQue.Clear();

        // 设置日志输出
        pt.SetLog((string info) => {
            PECommon.Log(info);
        });

        // 设置在主线程执行逻辑，将任务添加到一个队列里，之后再主线程去执行队列的的方法
        pt.SetHandle((Action<int> cb, int tid) => {
            if (cb != null) {
                lock (tpQueLock) {
                    tpQue.Enqueue(new TaskPack(tid, cb));
                }
            }
        });

        PECommon.Log("TimerSvc Init Done.");
    }

    /// <summary>
    /// 主线程执行队列里的逻辑
    /// </summary>
    public void Update() {
        while (tpQue.Count > 0) {
            // 取出
            TaskPack tp = null;
            lock (tpQueLock) {
                tp = tpQue.Dequeue(); 
            }
            // 执行
            if (tp != null) {
                tp.cb(tp.tid);
            }
        }
    }

    /// <summary>
    /// 添加 定时任务
    /// </summary>
    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int count = 1) {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }

    public long GetNowTime() {
        return (long)pt.GetMillisecondsTime();
    }
}