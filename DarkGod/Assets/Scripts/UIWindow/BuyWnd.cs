/****************************************************
    文件：BuyWnd.cs
	作者：CaptainYun
    日期：2019/5/26 20:45:55
	功能：购买交易窗口
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class BuyWnd : WindowRoot {
    public Text txtInfo;
    public Button btnSub;

    private int buyType; // 0：体力 1：金币
    public void SetBuyType(int type) {
        this.buyType = type;
    }

    protected override void InitWnd() {
        base.InitWnd();
        btnSub.interactable = true;
        RefreshUI();
    }

    private void RefreshUI() {
        switch (buyType) {
            case 0:
                // 体力
                txtInfo.text = "是否花费" + Constants.Color(" 10 钻石", TxtColor.Red) + "购买" + Constants.Color(" 100 体力", TxtColor.Green) + "?";
                break;
            case 1:
                // 金币
                txtInfo.text = "是否花费" + Constants.Color(" 10 钻石", TxtColor.Red) + "购买" + Constants.Color(" 1000 金币", TxtColor.Green) + "?";
                break;
        }
    }

    public void ClickSubBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        // 发送网络购买消息 
        GameMsg msg = new GameMsg {
            cmd = (int)CMD.ReqBuy,
            reqBuy = new ReqBuy {
                type = buyType,
                cost = 10
            }
        };
        netSvc.SendMsg(msg);
        btnSub.interactable = false;
    }

    public void ClickCloseBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetWndState(false);
    }
}