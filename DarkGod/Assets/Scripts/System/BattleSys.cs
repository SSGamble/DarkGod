/****************************************************
    文件：BattleSys.cs
	作者：CaptainYun
    日期：2019/6/1 22:30:42
	功能：战斗业务系统，负责全局的战斗业务
*****************************************************/

using PEProtocol;
using UnityEngine;

public class BattleSys : SystemRoot {

    public static BattleSys Instance = null;
    public PlayerCtrlWnd playerCtrlWnd;
    [HideInInspector]
    public BattleMgr battleMgr;
    public BattleEndWnd battleEndWnd;
    private int dungeonId;
    private double startTime;

    public override void InitSys() {
        base.InitSys();

        Instance = this;
        PECommon.Log("Init BattleSys...");
    }

    /// <summary>
    /// 开始战斗
    /// </summary>
    /// <param name="mapId"></param>
    public void StartBattle(int mapId) {
        dungeonId = mapId;
        GameObject go = new GameObject {
            name = "BattleRoot"
        };

        go.transform.SetParent(GameRoot.Instance.transform);
        battleMgr = go.AddComponent<BattleMgr>();
        battleMgr.Init(mapId,()=> {
            startTime = timerSvc.GetNowTime();
        });
        SetPlayerCtrlWndState();
    }

    public void SetPlayerCtrlWndState(bool isActive = true) {
        playerCtrlWnd.SetWndState(isActive);
    }

    public void SetBattleEndWndState(FBEndType type,bool isActive = true) {
        battleEndWnd.SetWndType(type);
        battleEndWnd.SetWndState(isActive);
    }

    public void SetMoveDir(Vector2 dir) {
        battleMgr.SetSelfPlayerMoveDir(dir);
    }

    public void ReqReleaseSkill(int index) {
        battleMgr.ReqReleaseSkill(index);
    }

    /// <summary>
    /// 获取方向的输入
    /// </summary>
    /// <returns></returns>
    public Vector2 GetDirInput() {
        return playerCtrlWnd.currentDir;
    }

    /// <summary>
    /// 战斗结束
    /// </summary>
    /// <param name="restHP">玩家剩余血量</param>
    public void EndBattle(bool isWin, int restHP) {
        playerCtrlWnd.SetWndState(false);
        GameRoot.Instance.dynamicWnd.RmvAllHpItemInfo();

        if (isWin) {
            double endTime = timerSvc.GetNowTime();
            // 发送结算战斗请求
            GameMsg msg = new GameMsg {
                cmd = (int)CMD.ReqDungeonFightEnd,
                reqDungeonFightEnd = new ReqDungeonFightEnd {
                    win = isWin,
                    dungeonId = dungeonId,
                    restHp = restHP,
                    costTime = (int)(endTime - startTime)
                }
            };
            netSvc.SendMsg(msg);
        }
        else {
            SetBattleEndWndState(FBEndType.Lose);
        }
    }

    /// <summary>
    /// 退出副本
    /// </summary>
    public void DestroyBattle() {
        SetPlayerCtrlWndState(false);
        SetBattleEndWndState(FBEndType.None, false);
        GameRoot.Instance.dynamicWnd.RmvAllHpItemInfo();
        Destroy(battleMgr.gameObject);
    }

    public void RspFightEnd(GameMsg msg) {
        RspDungeonFightEnd data = msg.rspDungeonFightEnd;
        GameRoot.Instance.SetPlayerDataByDungeonEnd(data);
        battleEndWnd.SetBattleEndData(data.dungeonId, data.costTime, data.restHp);
        SetBattleEndWndState(FBEndType.Win);
    }
}
