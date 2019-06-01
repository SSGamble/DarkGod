/****************************************************
    文件：DungeonWnd.cs
	作者：CaptainYun
    日期：2019/6/1 19:11:36
	功能：副本选择界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class DungeonWnd : WindowRoot {

    public Button[] dungeonBtnArr; // 所有副本按钮
    public Transform pointerTrans; // 当前副本的图标

    private PlayerData pd;

    protected override void InitWnd() {
        base.InitWnd();
        pd = GameRoot.Instance.PlayerData;
        RefreshUI();
    }

    public void RefreshUI() {
        int dungeonId = pd.dungeon;
        for (int i = 0; i < dungeonBtnArr.Length; i++) {
            if (i < dungeonId % 10000) { // 副本 ID：10001
                SetActive(dungeonBtnArr[i].gameObject);
                // 设置当前副本的显示位置
                if (i == dungeonId % 10000 - 1) {
                    pointerTrans.SetParent(dungeonBtnArr[i].transform);
                    pointerTrans.localPosition = new Vector3(25, 100, 0);
                }
            }
            else {
                SetActive(dungeonBtnArr[i].gameObject, false);
            }
        }
    }

    /// <summary>
    /// 进入副本
    /// </summary>
    public void ClickTaskBtn(int dungeonId) {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

        // 检查体力是否足够
        int power = resSvc.GetMapCfg(dungeonId).power; // 进入副本所需的体力
        if (power > pd.power) {
            GameRoot.AddTips("体力值不足");
        }
        else {
            // 请求开始战斗
            netSvc.SendMsg(new GameMsg {
                cmd = (int)CMD.ReqDungeonFight,
                reqDungeonFight = new ReqDungeonFight {
                    dungeonId = dungeonId
                }
            });
        }
    }

    public void ClickCloseBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetWndState(false);
    }
}