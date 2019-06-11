/****************************************************
    文件：BattleEndWnd.cs
	作者：CaptainYun
    日期：2019/6/10 19:17:9
	功能：战斗结算界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 副本界面的几种状态
/// </summary>
public enum FBEndType {
    None,
    Pause,
    Win,
    Lose
}

public class BattleEndWnd : WindowRoot {
    #region UI Define
    public Transform rewardTrans;
    public Button btnClose;
    public Button btnExit;
    public Button btnSure;
    public Text txtTime;
    public Text txtRestHP;
    public Text txtReward;
    public Animation ani;
    #endregion

    private FBEndType endType = FBEndType.None;

    protected override void InitWnd() {
        base.InitWnd();
        RefreshUI();
    }

    private void RefreshUI() {
        switch (endType) {
            case FBEndType.Pause:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject);
                break;
            case FBEndType.Win:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject, false);
                SetActive(btnClose.gameObject, false);
                MapCfg cfg = resSvc.GetMapCfg(dungeonId);
                int min = costTime / 60;
                int sec = costTime % 60;
                int coin = cfg.coin;
                int exp = cfg.exp;
                int crystal = cfg.crystal;
                SetText(txtTime, "通关时间：" + min + ":" + sec);
                SetText(txtRestHP, "剩余血量：" + restHp);
                SetText(txtReward, "关卡奖励：" + Constants.Color(coin + "金币", TxtColor.Green) + Constants.Color(exp + "经验", TxtColor.Yellow) + Constants.Color(crystal + "水晶", TxtColor.Blue));
                timerSvc.AddTimeTask((int tid) => {
                    SetActive(rewardTrans);
                    ani.Play();
                    // 分时间段播放音效组合
                    timerSvc.AddTimeTask((int tid1) => {
                        audioSvc.PlayUIAudio(Constants.FBItemEnter);
                        timerSvc.AddTimeTask((int tid2) => {
                            audioSvc.PlayUIAudio(Constants.FBItemEnter);
                            timerSvc.AddTimeTask((int tid3) => {
                                audioSvc.PlayUIAudio(Constants.FBItemEnter);
                                timerSvc.AddTimeTask((int tid5) => {
                                    audioSvc.PlayUIAudio(Constants.FBLogoEnter);
                                }, 200);
                            }, 200);
                        }, 200);
                    }, 200);
                }, 1000);
                break;
            case FBEndType.Lose:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject, false);
                audioSvc.PlayUIAudio(Constants.FBLose);
                break;
        }
    }

    public void ClickClose() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        BattleSys.Instance.battleMgr.isPauseGame = false;
        SetWndState(false);
    }

    public void ClickExitBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        // 进入主城，销毁当前战斗
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
    }

    public void ClickSureBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        // 进入主城，销毁当前战斗
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
        // 打开副本界面
        DungeonSys.Instance.EnterDungeon();
    }

    public void SetWndType(FBEndType endType) {
        this.endType = endType;
    }

    private int dungeonId;
    private int costTime;
    private int restHp;
    public void SetBattleEndData(int dungeonId, int costTime, int restHp) {
        this.dungeonId = dungeonId;
        this.costTime = costTime;
        this.restHp = restHp;
    }
}

