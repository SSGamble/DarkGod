/****************************************************
	文件：MapMgr.cs
	作者：CaptainYun
	日期：2019/06/02 10:20   	
	功能：地图管理器
*****************************************************/

using UnityEngine;

public class MapMgr : MonoBehaviour {

    private BattleMgr battleMgr;
    private int waveIndex = 1; // 默认生成第一波怪物

    public void Init(BattleMgr battle) {
        battleMgr = battle;
        //实例化第一批怪物
        battleMgr.LoadMonsterByWaveID(waveIndex);
        PECommon.Log("Init MapMgr Done.");
    }
}
