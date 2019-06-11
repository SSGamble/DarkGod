/****************************************************
    文件：DungeonSys.cs
	作者：CaptainYun
    日期：2019/6/1 19:14:31
	功能：副本业务系统
*****************************************************/

using PEProtocol;
using UnityEngine;

public class DungeonSys : SystemRoot {
    public static DungeonSys Instance = null;

    public DungeonWnd dungeonWnd;

    public override void InitSys() {
        base.InitSys();

        Instance = this;
        PECommon.Log("Init DungeonSys...");
    }

    public void EnterDungeon() {
        SetDungeonWndState();
    }

    public void SetDungeonWndState(bool isActive=true) {
        dungeonWnd.SetWndState(isActive);
    }

    public void RspDungeonFight(GameMsg msg) {
        GameRoot.Instance.SetPlayerDataByDungeonStart(msg.rspDungeonFight);
        MainCitySys.Instance.mainCityWnd.SetWndState(false);
        SetDungeonWndState(false);
        BattleSys.Instance.StartBattle(msg.rspDungeonFight.dungeonId);
    }

}
