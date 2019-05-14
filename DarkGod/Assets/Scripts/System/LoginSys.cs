/****************************************************
    文件：LoginSys.cs
	作者：CaptainYun
    日期：2019/5/12 21:47:25
	功能：登录注册业务系统
*****************************************************/

using System;
using UnityEngine;

public class LoginSys : SystemRoot {

    public static LoginSys Instance = null;
    public LoginWnd loginWnd;
    public CreateWnd createWnd;

    public override void InitSys() {
        base.InitSys();
        Instance = this;
        Debug.Log("Init LoginSys..");
    }

    /// <summary>
    /// 进入登录场景
    /// </summary>
    public void EnterLogin() {
        // 异步的加载登录场景
        resSvc.AsyncLoadScene(Constants.SceneLogin,()=> {
            // 加载完成后再打开注册登录界面
            loginWnd.SetWndState();
            // 播放背景音乐
            audioSvc.PlayBGAudio(Constants.BGLogin);
        });
    }

    /// <summary>
    /// 客户端登录相应
    /// </summary>
    public void RspLogin() {
        GameRoot.AddTips("登录成功");
        //打开角色创建窗口
        createWnd.SetWndState();
        //关闭登录界面
        loginWnd.SetWndState(false);
    }
}