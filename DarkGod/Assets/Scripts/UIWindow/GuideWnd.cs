/****************************************************
    文件：GuideWnd.cs
	作者：CaptainYun
    日期：2019/5/24 16:21:21
	功能：引导对话界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class GuideWnd : WindowRoot 
{
    public Text txtName;
    public Text txtTalk;
    public Image imgIcon;

    private PlayerData pd;
    private AutoGuideCfg curtTaskData; // 任务数据
    private string[] dialogArr; // 对话数组
    private int index; // 对话索引，进行到了哪一条对话
     
    protected override void InitWnd() {
        base.InitWnd();

        pd = GameRoot.Instance.PlayerData;
        curtTaskData = MainCitySys.Instance.GetCurtTaskData();
        dialogArr = curtTaskData.dilogArr.Split('#');
        index = 1;

        SetTalk();
    }

    /// <summary>
    /// 设置对话内容
    /// </summary>
    private void SetTalk() {
        string[] talkArr = dialogArr[index].Split('|');
        
        if (talkArr[0] == "0") { // 自己
            SetSprite(imgIcon, PathDefine.SelfIcon);
            SetText(txtName, pd.name);
        }
        else {  // 对话 NPC
            switch (curtTaskData.npcID) {
                case 0:
                    SetSprite(imgIcon, PathDefine.WiseManIcon);
                    SetText(txtName, "智者");
                    break;
                case 1:
                    SetSprite(imgIcon, PathDefine.GeneralIcon);
                    SetText(txtName, "将军");
                    break;
                case 2:
                    SetSprite(imgIcon, PathDefine.ArtisanIcon);
                    SetText(txtName, "工匠");
                    break;
                case 3:
                    SetSprite(imgIcon, PathDefine.TraderIcon);
                    SetText(txtName, "商人");
                    break;
                default:
                    SetSprite(imgIcon, PathDefine.GuideIcon);
                    SetText(txtName, "小芸");
                    break;
            }
        }
        imgIcon.SetNativeSize();
        SetText(txtTalk, talkArr[1].Replace("$name", pd.name)); // 替换玩家名字
    }

    /// <summary>
    /// 下一步
    /// </summary>
    public void ClickNextBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

        index += 1;
        // 对话结束
        if (index == dialogArr.Length) {
            // 发送任务引导完成信息
            GameMsg msg = new GameMsg {
                cmd = (int)CMD.ReqGuide,
                reqGuide = new ReqGuide {
                    guideid = curtTaskData.id
                }
            };

            netSvc.SendMsg(msg);
            SetWndState(false);
        }
        else {
            SetTalk();
        }
    }
}