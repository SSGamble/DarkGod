/****************************************************
    文件：WindowRoot.cs
	作者：CaptainYun
    日期：2019/5/13 21:6:22
	功能：UI 界面基类
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

public class WindowRoot : MonoBehaviour {

    protected ResSvc resSvc = null;
    protected AudioSvc audioSvc = null;

    /// <summary>
    /// 设置窗口的激活状态
    /// </summary>
    /// <param name="isActive">是否激活，默认 true</param>
    public void SetWndState(bool isActive = true) {
        //设置激活状态
        if (gameObject.activeSelf != isActive) {
            SetActive(gameObject, isActive);
        }
        //如果是激活窗口，则进行窗口的初始化
        if (isActive) {
            InitWnd();
        }
        //如果是禁用窗口，则进行窗口的清理
        else {
            ClearWnd();
        }
    }

    /// <summary>
    /// 初始化窗口
    /// </summary>
    protected virtual void InitWnd() {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
    }

    /// <summary>
    /// 清理窗口
    /// </summary>
    protected virtual void ClearWnd() {
        resSvc = null;
        audioSvc = null;
    }

    #region 设置 Text 组件的文字
    protected void SetText(Text txt, string context = "") {
        txt.text = context;
    }

    protected void SetText(Text txt, int num = 0) {
        SetText(txt, num.ToString());
    }

    // 获取 Transform 上的 Text 组件上的文字
    protected void SetText(Transform trans, int num = 0) {
        SetText(trans.GetComponent<Text>(), num);
    }

    protected void SetText(Transform trans, string context = "") {
        SetText(trans.GetComponent<Text>(), context);
    }
    #endregion

    #region 激活物体
    protected void SetActive(GameObject go, bool isActive = true) {
        go.SetActive(isActive);
    }
    protected void SetActive(Transform trans, bool state = true) {
        trans.gameObject.SetActive(state);
    }
    protected void SetActive(RectTransform rectTrans, bool state = true) {
        rectTrans.gameObject.SetActive(state);
    }
    protected void SetActive(Image img, bool state = true) {
        img.transform.gameObject.SetActive(state);
    }
    protected void SetActive(Text txt, bool state = true) {
        txt.transform.gameObject.SetActive(state);
    }
    #endregion
}