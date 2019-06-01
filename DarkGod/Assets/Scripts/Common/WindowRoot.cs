/****************************************************
    文件：WindowRoot.cs
	作者：CaptainYun
    日期：2019/5/13 21:6:22
	功能：UI 界面基类
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowRoot : MonoBehaviour {

    protected ResSvc resSvc = null;
    protected AudioSvc audioSvc = null;
    protected NetSvc netSvc = null;
    protected TimerSvc timerSvc = null;

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
        netSvc = NetSvc.Instance;
        timerSvc = TimerSvc.Instance;
    }

    /// <summary>
    /// 清理窗口
    /// </summary>
    protected virtual void ClearWnd() {
        resSvc = null;
        audioSvc = null;
        netSvc = null;
        timerSvc = null;
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

    #region 点击事件
    // 为指定物体添加事件监听脚本并设置回调
    protected void OnClickDown(GameObject go, Action<PointerEventData> cb) {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onClickDown = cb;
    }
    protected void OnClickUp(GameObject go, Action<PointerEventData> cb) {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onClickUp = cb;
    }
    protected void OnClickDrag(GameObject go, Action<PointerEventData> cb) {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onDrag = cb;
    }
    /// <summary>
    /// 带参数点击
    /// </summary>
    /// <param name="go">响应物体</param>
    /// <param name="cb">回调</param>
    /// <param name="args">传递参数</param>
    protected void OnClick(GameObject go, Action<object> cb, object args) {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onClick = cb;
        listener.args = args;
    }
    #endregion

    /// <summary>
    /// 设置图片
    /// </summary>
    protected void SetSprite(Image img, string path) {
        Sprite sp = resSvc.LoadSprite(path, true);
        img.sprite = sp;
    }

    /// <summary>
    /// 为物体添加组件，如果已有该组件就获取
    /// where: T 必须要是组件的子类才能被添加
    /// </summary>
    protected T GetOrAddComponect<T>(GameObject go) where T : Component {
        T t = go.GetComponent<T>();
        if (t == null) {
            t = go.AddComponent<T>();
        }
        return t;
    }

    /// <summary>
    /// 判断当前物体是否激活
    /// </summary>
    public bool GetWndState() {
        return gameObject.activeSelf;
    }

    /// <summary>
    /// 查找指定父物体下的子物体
    /// </summary>
    /// <param name="trans">父物体</param>
    /// <param name="name">要查找的子物体的名字</param>
    protected Transform GetTrans(Transform trans, string name) {
        if (trans != null) {
            return trans.Find(name);
        }
        else {
            return transform.Find(name);
        }
    }
}