/****************************************************
    文件：BattleSys.cs
	作者：CaptainYun
    日期：2019/6/1 22:30:42
	功能：战斗业务系统，负责全局的战斗业务
*****************************************************/

using UnityEngine;

public class BattleSys : SystemRoot {
    public static BattleSys Instance = null;
    public PlayerCtrlWnd playerCtrlWnd;
    public BattleMgr battleMgr;

    public override void InitSys() {
        base.InitSys();

        Instance = this;
        PECommon.Log("Init BattleSys...");
    }

    public void StartBattle(int mapId) {
        GameObject go = new GameObject {
            name = "BattleRoot"
        };

        go.transform.SetParent(GameRoot.Instance.transform);
        battleMgr = go.AddComponent<BattleMgr>();
        battleMgr.Init(mapId);

        SetPlayerCtrlWndState();
    }

    public void SetPlayerCtrlWndState(bool isActive = true) {
        playerCtrlWnd.SetWndState(isActive);
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
}
