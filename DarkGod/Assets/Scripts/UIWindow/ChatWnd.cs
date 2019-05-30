/****************************************************
    文件：ChatWnd.cs
	作者：CaptainYun
    日期：2019/5/26 15:13:49
	功能：聊天界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChatWnd : WindowRoot {
    public InputField iptChat;
    public Text txtChat;
    public Image imgWorld;
    public Image imgGuild;
    public Image imgFriend;

    private int chatType; // 世界 0，工会 1，好友  2
    private List<string> chatLst = new List<string>(); // 聊天信息
    private bool canSend = true; // 用于聊天时间间隙

    protected override void InitWnd() {
        base.InitWnd();
        chatType = 0;
        RefreshUI();
    }

    private void RefreshUI() {
        // 世界
        if (chatType == 0) { 
            string chatMsg = "";
            for (int i = 0; i < chatLst.Count; i++) {
                chatMsg += chatLst[i] + "\n"; // 因为只使用了一个 text ，所以一条消息后换行
            }
            SetText(txtChat, chatMsg);
            SetSprite(imgWorld, "ResImages/btntype1");
            SetSprite(imgGuild, "ResImages/btntype2");
            SetSprite(imgFriend, "ResImages/btntype2");
        }
        // 工会
        else if (chatType == 1) {
            SetText(txtChat, "尚未加入公会");
            SetSprite(imgWorld, "ResImages/btntype2");
            SetSprite(imgGuild, "ResImages/btntype1");
            SetSprite(imgFriend, "ResImages/btntype2");
        }
        // 好友
        else if (chatType == 2) {
            SetText(txtChat, "暂无好友信息");
            SetSprite(imgWorld, "ResImages/btntype2");
            SetSprite(imgGuild, "ResImages/btntype2");
            SetSprite(imgFriend, "ResImages/btntype1");
        }
    }


    /// <summary>
    /// 接收服务器推送后显示聊天信息
    /// </summary>
    public void AddChatMsg(string name, string chat) {
        chatLst.Add(Constants.Color(name + "：", TxtColor.Blue) + chat);
        if (chatLst.Count > 12) {
            chatLst.RemoveAt(0);
        }
        if (GetWndState()) {
            RefreshUI();
        }
    }

    public void ClickSendBtn() {
        if (!canSend) {
            GameRoot.AddTips("聊天消息每 5 秒钟才能发送一条");
            return;
        }

        if (iptChat.text != null && iptChat.text != "" && iptChat.text != " ") {
            if (iptChat.text.Length > 12) {
                GameRoot.AddTips("输入信息不能超过 12 个字");
            }
            else {
                // 发送网络消息到服务器
                GameMsg msg = new GameMsg {
                    cmd = (int)CMD.SndChat,
                    sndChat = new SndChat {
                        chat = iptChat.text
                    }
                };
                iptChat.text = "";
                netSvc.SendMsg(msg);
                canSend = false;

                // 定时任务
                timerSvc.AddTimeTask((int tid) => {
                    canSend = true;
                }, 5, PETimeUnit.Second);
            }
        }
        else {
            GameRoot.AddTips("尚未输入聊天信息");
        }
    }

    public void ClickWorldBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = 0;
        RefreshUI();
    }
    public void ClickGuildBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = 1;
        RefreshUI();
    }
    public void ClickFriendBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = 2;
        RefreshUI();
    }
    public void ClickCloseBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = 0;
        SetWndState(false);
    }
}