/****************************************************
	文件：PETimer.cs
	作者：CaptainYun
	日期：2019/05/29 21:26   	
	功能：计时器
*****************************************************/
using System;
using System.Collections.Generic;
using System.Timers;

public class PETimer {
    // 日志
    private Action<string> taskLog;
    public void SetLog(Action<string> log) {
        taskLog = log;
    }

    private Action<Action<int>, int> taskHandle; // 执行的回调 / tid
    private Timer srvTimer; // 用于独立线程
    private double nowTime; // 当前时间

    private DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0); // 计算机元年
    private static readonly string lockTid = "lockTid"; // 锁
    private int tid; // 全局 id
    private List<int> tidLst = new List<int>(); // 所有的 tid
    private List<int> recTidLst = new List<int>(); // 需要回收的 tid 列表

    private static readonly string lockTime = "lockTime";
    private List<TimeTask> taskTimeLst = new List<TimeTask>();  // 计时任务列表
    private List<TimeTask> tmpTimeLst = new List<TimeTask>(); // 临时缓存
    private List<int> tmpDelTimeLst = new List<int>(); // 临时删除列表

    private int frameCounter; // 帧计数
    private static readonly string lockFrame = "lockFrame";
    private List<FrameTask> taskFrameLst = new List<FrameTask>();  // 帧任务列表
    private List<FrameTask> tmpFrameLst = new List<FrameTask>(); // 临时缓存
    private List<int> tmpDelFrameLst = new List<int>(); // 临时删除列表

    public void Update() {
        CheckTimeTask();
        CheckFrameTask();
        DelTimeTask();
        DelFrameTask();
        // 回收
        if (recTidLst.Count > 0) {
            lock (lockTid) {
                RecycleTid();
            }
        }
    }

    public PETimer(int interval = 0) {
        tidLst.Clear();
        recTidLst.Clear();
        taskTimeLst.Clear();
        tmpTimeLst.Clear();
        taskFrameLst.Clear();
        tmpFrameLst.Clear();

        if (interval != 0) {
            srvTimer = new Timer(interval);
            srvTimer.AutoReset = true; // 自动循环
            srvTimer.Elapsed += (object sender, ElapsedEventArgs args) => {
                Update();
            };
            srvTimer.Start();
        }
    }

    /// <summary>
    /// 检测时间定时任务
    /// </summary>
    private void CheckTimeTask() {
        if (tmpTimeLst.Count > 0) {
            lock (lockTime) { // 大部分时间都不会跑到这里来
                // 将缓存区中的定时任务加入任务列表
                for (int tmpIndex = 0; tmpIndex < tmpTimeLst.Count; tmpIndex++) {
                    taskTimeLst.Add(tmpTimeLst[tmpIndex]);
                }
                // 清空临时缓存
                tmpTimeLst.Clear();
            }
        }
        nowTime = GetUTCMilliseconds();
        // 遍历检测任务是否达到时间条件
        for (int index = 0; index < taskTimeLst.Count; index++) {
            TimeTask task = taskTimeLst[index];
            // 时间没到
            if (nowTime.CompareTo(task.destTime) < 0) {
                continue;
            }
            // 已到目标时间，执行任务后移除
            else {
                Action<int> cb = task.callback;
                try {
                    if (taskHandle != null) {
                        taskHandle(cb, task.tid);
                    }
                    else {
                        if (cb != null) {
                            cb(task.tid);
                        }
                    }
                }
                catch (Exception e) {
                    LogInfo(e.ToString());
                }
                // 移除已完成的任务
                if (task.count == 1) {
                    taskTimeLst.RemoveAt(index);
                    index--;
                    recTidLst.Add(task.tid); // 需要回收
                }
                else {
                    if (task.count != 0) { // 不是循环执行
                        task.count -= 1;
                    }
                    task.destTime += task.delay; // 重新设置目标时间（下一次执行任务的时间）
                }
            }
        }
    }

    /// <summary>
    /// 检测帧定时任务
    /// </summary>
    private void CheckFrameTask() {
        if (tmpFrameLst.Count > 0) {
            lock (lockFrame) {
                for (int tmpIndex = 0; tmpIndex < tmpFrameLst.Count; tmpIndex++) {
                    taskFrameLst.Add(tmpFrameLst[tmpIndex]);
                }
                // 清空临时缓存
                tmpFrameLst.Clear();
            }
        }

        // 将缓存区中的定时任务加入任务列表

        // 计时，帧 +1
        frameCounter += 1;
        // 遍历检测任务是否达到时间条件
        for (int index = 0; index < taskFrameLst.Count; index++) {
            FrameTask task = taskFrameLst[index];
            // 目标帧没到
            if (frameCounter < task.destFrame) {
                continue;
            }
            // 已到目标帧，执行任务后移除
            else {
                Action<int> cb = task.callback;
                try {
                    if (taskHandle != null) {
                        taskHandle(cb, task.tid);
                    }
                    else {
                        if (cb != null) {
                            cb(task.tid);
                        }
                    }
                }
                catch (Exception e) {
                    LogInfo(e.ToString());
                }
                // 移除已完成的任务
                if (task.count == 1) {
                    taskFrameLst.RemoveAt(index);
                    index--;
                    recTidLst.Add(task.tid); // 需要回收
                }
                else {
                    if (task.count != 0) { // 不是循环执行
                        task.count -= 1;
                    }
                    task.destFrame += task.delay; // 重新设置目标帧（下一次执行任务的时间）
                }
            }
        }
    }

    /// <summary>
    /// 删除 临时 时间定时任务 列表
    /// </summary>
    private void DelTimeTask() {
        if (tmpDelTimeLst.Count > 0) {
            lock (lockTime) {
                for (int i = 0; i < tmpDelTimeLst.Count; i++) {
                    bool isDel = false;
                    int delTid = tmpDelTimeLst[i];
                    // 任务存在任务列表里
                    for (int j = 0; j < taskTimeLst.Count; j++) {
                        TimeTask task = taskTimeLst[j];
                        if (task.tid == delTid) {
                            isDel = true;
                            taskTimeLst.RemoveAt(j);
                            recTidLst.Add(delTid);
                            //LogInfo("Del taskTimeLst ID:" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
                            break;
                        }
                    }
                    if (isDel) continue;
                    // 任务不存在任务列表里，去临时缓存里处理
                    for (int j = 0; j < tmpTimeLst.Count; j++) {
                        TimeTask task = tmpTimeLst[j];
                        if (task.tid == delTid) {
                            tmpTimeLst.RemoveAt(j);
                            recTidLst.Add(delTid);
                            //LogInfo("Del tmpTimeLst ID:" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
                            break;
                        }
                    }
                }
                tmpDelTimeLst.Clear();
            }
        }
    }

    private void DelFrameTask() {
        if (tmpDelFrameLst.Count > 0) {
            lock (lockFrame) {
                for (int i = 0; i < tmpDelFrameLst.Count; i++) {
                    bool isDel = false;
                    int delTid = tmpDelFrameLst[i];
                    for (int j = 0; j < taskFrameLst.Count; j++) {
                        FrameTask task = taskFrameLst[j];
                        if (task.tid == delTid) {
                            isDel = true;
                            taskFrameLst.RemoveAt(j);
                            recTidLst.Add(delTid);
                            break;
                        }
                    }
                    if (isDel) continue;
                    for (int j = 0; j < tmpFrameLst.Count; j++) {
                        FrameTask task = tmpFrameLst[j];
                        if (task.tid == delTid) {
                            tmpFrameLst.RemoveAt(j);
                            recTidLst.Add(delTid);
                            break;
                        }
                    }
                }
                tmpDelFrameLst.Clear();
            }
        }
    }

    #region 时间任务
    /// <summary>
    /// 增加定时任务，可能是多线程操作
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="delay">延迟时间</param>
    /// <param name="PETimeUnit">时间单位</param>
    /// <param name="count">循环次数</param>
    /// <returns>任务的全局 ID</returns>
    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit PETimeUnit = PETimeUnit.Millisecond, int count = 1) {
        if (PETimeUnit != PETimeUnit.Millisecond) {
            switch (PETimeUnit) {
                case PETimeUnit.Second:
                    delay = delay * 1000;
                    break;
                case PETimeUnit.Minute:
                    delay = delay * 1000 * 60;
                    break;
                case PETimeUnit.Hour:
                    delay = delay * 1000 * 60 * 60;
                    break;
                case PETimeUnit.Day:
                    delay = delay * 1000 * 60 * 60 * 24;
                    break;
                default:
                    LogInfo("Add Task PETimeUnit Type Error...");
                    break;
            }
        }
        int tid = GetTid(); // 获取全局唯一 ID
        nowTime = GetUTCMilliseconds();
        lock (lockTime) {
            tmpTimeLst.Add(new TimeTask(tid, callback, nowTime + delay, delay, count)); // 添加到临时缓存
        }
        return tid;
    }

    /// <summary>
    /// 删除任务
    /// </summary>
    public void DelTimeTask(int tid) {
        lock (lockTime) {
            tmpDelTimeLst.Add(tid);
            LogInfo("TmpDel ID:" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
        }
    }

    /// <summary>
    /// 替换任务
    /// </summary>
    /// <param name="tid"></param>
    /// <returns></returns>
    public bool ReplaceTimeTask(int tid, Action<int> callback, float delay, PETimeUnit PETimeUnit = PETimeUnit.Millisecond, int count = 1) {

        if (PETimeUnit != PETimeUnit.Millisecond) {
            switch (PETimeUnit) {
                case PETimeUnit.Second:
                    delay = delay * 1000;
                    break;
                case PETimeUnit.Minute:
                    delay = delay * 1000 * 60;
                    break;
                case PETimeUnit.Hour:
                    delay = delay * 1000 * 60 * 60;
                    break;
                case PETimeUnit.Day:
                    delay = delay * 1000 * 60 * 60 * 24;
                    break;
                default:
                    LogInfo("Add Task PETimeUnit Type Error...");
                    break;
            }
        }
        nowTime = GetUTCMilliseconds();
        TimeTask newTask = new TimeTask(tid, callback, nowTime + delay, delay, count);

        bool isRep = false; // 是否替换成功
        // 尝试在任务列表里寻找替换
        for (int i = 0; i < taskTimeLst.Count; i++) {
            if (taskTimeLst[i].tid == tid) {
                taskTimeLst[i] = newTask;
                isRep = true;
                break;
            }
        }
        // 任务列表里没有，尝试去缓存找
        if (!isRep) {
            for (int i = 0; i < tmpTimeLst.Count; i++) {
                if (tmpTimeLst[i].tid == tid) {
                    tmpTimeLst[i] = newTask;
                    isRep = true;
                    break;
                }
            }
        }

        return isRep;
    }

    #endregion

    #region 帧的定时任务
    /// <summary>
    /// 增加定时任务，可能是多线程操作
    /// </summary>
    public int AddFrameTask(Action<int> callback, int delay, int count = 1) {
        int tid = GetTid(); // 获取全局唯一 ID
        lock (lockFrame) {
            tmpFrameLst.Add(new FrameTask(tid, callback, frameCounter + delay, delay, count)); // 添加到临时缓存
        }
        LogInfo("Test");
        return tid;
    }

    /// <summary>
    /// 删除任务
    /// </summary>
    public void DelFrameTask(int tid) {
        lock (lockFrame) {
            tmpDelFrameLst.Add(tid);
        }
    }

    /// <summary>
    /// 替换任务
    /// </summary>
    public bool ReplaceFrameTask(int tid, Action<int> callback, int delay, int count = 1) {
        FrameTask newTask = new FrameTask(tid, callback, frameCounter + delay, delay, count);

        bool isRep = false; // 是否替换成功
        // 尝试在任务列表里寻找替换
        for (int i = 0; i < taskFrameLst.Count; i++) {
            if (taskFrameLst[i].tid == tid) {
                taskFrameLst[i] = newTask;
                isRep = true;
                break;
            }
        }
        // 任务列表里没有，尝试去缓存找
        if (!isRep) {
            for (int i = 0; i < tmpFrameLst.Count; i++) {
                if (tmpFrameLst[i].tid == tid) {
                    tmpFrameLst[i] = newTask;
                    isRep = true;
                    break;
                }
            }
        }

        return isRep;
    }

    #endregion

    #region 工具函数
    /// <summary>
    /// 获取全局id，线程安全的
    /// </summary>
    /// <returns></returns>
    private int GetTid() {
        lock (lockTid) {
            tid += 1;
            while (true) {
                // 防止溢出
                if (tid == int.MaxValue) {
                    tid = 0;
                }
                // 检查是否被使用
                bool used = false;
                for (int i = 0; i < tidLst.Count; i++) {
                    if (tid == tidLst[i]) {
                        used = true;
                        break;
                    }
                }
                if (!used) { // 没有被使用
                    tidLst.Add(tid);
                    break;
                }
                else { // 已经被使用了
                    tid += 1;
                }
            }
        }
        return tid;
    }

    /// <summary>
    /// 回收 Tid
    /// </summary>
    private void RecycleTid() {
        for (int i = 0; i < recTidLst.Count; i++) {
            int tid = recTidLst[i];
            for (int j = 0; j < tidLst.Count; j++) {
                if (tidLst[j] == tid) {
                    tidLst.RemoveAt(j);
                    break;
                }
            }
        }
        recTidLst.Clear();
    }

    /// <summary>
    /// 打印日志
    /// </summary>
    private void LogInfo(string info) {
        if (taskLog != null) {
            taskLog(info);
        }
    }

    /// <summary>
    /// 从现在到计算机元年的毫秒数
    /// </summary>
    /// <returns></returns>
    private double GetUTCMilliseconds() {
        TimeSpan ts = DateTime.UtcNow - startDateTime;
        return ts.TotalMilliseconds;
    }

    public void Reset() {
        tid = 0;
        tidLst.Clear();
        recTidLst.Clear();
        taskTimeLst.Clear();
        tmpTimeLst.Clear();
        taskFrameLst.Clear();
        tmpFrameLst.Clear();
        taskLog = null;
        srvTimer.Stop();
    }

    /// <summary>
    /// 把定时任务的执行放到一个回调里面
    /// </summary>
    public void SetHandle(Action<Action<int>, int> handle) {
        taskHandle = handle;
    }

    /// <summary>
    /// 获取当前时间，计算机元年
    /// </summary>
    public double GetMillisecondsTime() {
        return nowTime;
    }

    /// <summary>
    /// 获取本地时间
    /// </summary>
    public DateTime GetLocalDateTime() {
        DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(startDateTime.AddMilliseconds(nowTime));
        return dt;
    }

    /// <summary>
    /// 时间格式化
    /// </summary>
    private string GetTimeStr(int time) {
        if (time < 10) {
            return "0" + time;
        }
        else {
            return time.ToString();
        }
    }

    /// <summary>
    /// 获取本地时间 - 字符串
    /// </summary>
    /// <returns></returns>
    public string GetLocalTimeStr() {
        DateTime dt = GetLocalDateTime();
        string str = GetTimeStr(dt.Hour) + ":" + GetTimeStr(dt.Minute) + ":" + GetTimeStr(dt.Second);
        return str;
    }

    public int GetYear() {
        return GetLocalDateTime().Year;
    }
    public int GetMonth() {
        return GetLocalDateTime().Month;
    }
    public int GetDay() {
        return GetLocalDateTime().Day;
    }
    public int GetWeek() {
        return (int)GetLocalDateTime().DayOfWeek;
    }
    #endregion
}

/// <summary>
/// 时间单位
/// </summary>
public enum PETimeUnit {
    Millisecond,
    Second,
    Minute,
    Hour,
    Day
}

/// <summary>
/// 时间定时
/// </summary>
public class TimeTask {

    public int tid; // 全局唯一 ID
    public double destTime; // 目标时间，毫秒
    public Action<int> callback; // 要执行什么
    public double delay; // 延迟时间
    public int count; // 执行次数，0：一直循环执行

    public TimeTask(int tid, Action<int> callback, double destTime, double delay, int count) {
        this.tid = tid;
        this.callback = callback;
        this.destTime = destTime;
        this.delay = delay;
        this.count = count;
    }
}

/// <summary>
/// 帧定时
/// </summary>
public class FrameTask {
    public int tid;
    public Action<int> callback;
    public int destFrame; // 目标帧
    public int delay;
    public int count;

    public FrameTask(int tid, Action<int> callback, int destFrame, int delay, int count) {
        this.tid = tid;
        this.callback = callback;
        this.destFrame = destFrame;
        this.delay = delay;
        this.count = count;
    }
}