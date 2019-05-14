/****************************************************
    文件：CreateWnd.cs
	作者：CaptainYun
    日期：2019/5/14 12:12:23
	功能：角色创建界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class CreateWnd : WindowRoot {

    public InputField iptName;

    protected override void InitWnd() {
        base.InitWnd();

        //随机一个名字
        iptName.text = resSvc.GetRanName(false);
    }

    /// <summary>
    /// 随机名字按钮点击事件
    /// </summary>
    public void ClickRanBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        iptName.text = resSvc.GetRanName(false);
    }

    /// <summary>
    /// 进入游戏按钮点击事件
    /// </summary>
    public void ClickEnterBtn() {
        audioSvc.PlayUIAudio(Constants.UILoginBtn);
        if (iptName.text != "") {
            //TODO
            //发送名字数据到服务器，登录主城
        }
        else {
            GameRoot.AddTips("当前名字不符合规范");
        }
    }
}