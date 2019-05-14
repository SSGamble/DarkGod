/****************************************************
    文件：LoginWnd.cs
	作者：CaptainYun
    日期：2019/5/13 11:56:2
	功能：登录注册界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LoginWnd : WindowRoot {
    public Button btnLogin;
    public Button btnNotice;
    public InputField iptAcct;
    public InputField iptPwd;

    /// <summary>
    /// 初始化窗口信息
    /// </summary>
    protected override void InitWnd() {
        base.InitWnd();
        //获取本地存储的账号密码
        if (PlayerPrefs.HasKey("Acct") && PlayerPrefs.HasKey("Pwd")) { // 判断是否存在账号和密码
            iptAcct.text = PlayerPrefs.GetString("Acct");
            iptPwd.text = PlayerPrefs.GetString("Pwd");
        }
        else {
            iptAcct.text = "";
            iptPwd.text = "";
        }
    }

    /// <summary>
    /// 点击进入游戏按钮
    /// </summary>
    public void ClickEnterButton() {
        audioSvc.PlayUIAudio(Constants.UILoginBtn);
        string acct = iptAcct.text;
        string pwd = iptPwd.text;
        // 输入合法
        if (acct != "" && pwd != "") {
            //更新本地存储的账号和密码
            PlayerPrefs.SetString("Acct", acct);
            PlayerPrefs.SetString("Pwd", pwd);

            //TODO 发送网络消息，请求登录
            LoginSys.Instance.RspLogin();
        }
        // 输入不合法
        else {
            GameRoot.AddTips("输入不合法");
        }
    }

    /// <summary>
    /// 公告按钮点击事件
    /// </summary>
    public void ClickNoticeBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        GameRoot.AddTips("功能正在开发中...");
    }
}