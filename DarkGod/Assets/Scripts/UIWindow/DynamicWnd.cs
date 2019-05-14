/****************************************************
    文件：DynamicWnd.cs
	作者：CaptainYun
    日期：2019/5/13 22:26:41
	功能：动态 UI 元素界面
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWnd : WindowRoot {
    public Animation tipsAni;
    public Text txtTips;

    //用于显示 tips 的队列，按序逐条显示
    private Queue<string> tipsQue = new Queue<string>();
    //是否正在显示 tips
    private bool isTipsShow = false;

    /// <summary>
    /// 将 Tips 压入栈
    /// </summary>
    /// <param name="tips"></param>
    public void EnqTips(string tips) {
        lock (tipsQue) {
            tipsQue.Enqueue(tips);
        }
    }

    private void Update() {
        // 逐条显示队列里的 Tips
        if (tipsQue.Count > 0 && isTipsShow == false) {
            lock (tipsQue) {
                string tips = tipsQue.Dequeue();
                isTipsShow = true;
                SetTips(tips);
            }
        }
    }

    /// <summary>
    /// 初始化窗口信息
    /// </summary>
    protected override void InitWnd() {
        base.InitWnd();
        SetActive(txtTips, false); // 默认关闭提示文字的显示
    }

    /// <summary>
    /// 设置 Tip 的文字，并显示
    /// </summary>
    /// <param name="tips">要显示的文字</param>
    private void SetTips(string tips) {
        SetActive(txtTips);
        SetText(txtTips, tips);

        // 播放动画
        AnimationClip aniClip = tipsAni.GetClip("TipsShowAni");
        tipsAni.Play();

        // 延时关闭激活
        StartCoroutine(AniPlayDone(aniClip.length, () => {
            SetActive(txtTips, false);
            isTipsShow = false;
        }));
    }

    /// <summary>
    /// 延时执行方法
    /// </summary>
    /// <param name="sec">等待时间</param>
    /// <param name="cb">方法回调</param>
    /// <returns></returns>
    IEnumerator AniPlayDone(float sec, Action cb) {
        yield return new WaitForSeconds(sec);
        if (cb != null) {
            cb();
        }
    }
}