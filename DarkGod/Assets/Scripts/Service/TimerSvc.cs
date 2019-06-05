/****************************************************
    文件：TimerSvc.cs
	作者：CaptainYun
    日期：2019/5/30 21:32:15
	功能：计时服务
*****************************************************/

using System;

public class TimerSvc : SystemRoot {
    public static TimerSvc Instance = null;

    private PETimer pt;

    public void InitSvc() {
        Instance = this;
        pt = new PETimer();

        // 设置日志输出
        pt.SetLog((string info) => {
            PECommon.Log(info);
        });

        PECommon.Log("Init TimerSvc...");
    }

    public void Update() {
        pt.Update(); // 驱动整个定时任务的检测
    }

    /// <summary>
    /// 添加定时任务
    /// </summary>
    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int count = 1) {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }

    /// <summary>
    /// 获取当前时间
    /// </summary>
    public double GetNowTime() {
        return pt.GetMillisecondsTime();
    }
}