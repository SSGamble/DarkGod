/****************************************************
	文件：MapMgr.cs
	作者：CaptainYun
	日期：2019/06/02 10:20   	
	功能：地图管理器
*****************************************************/

using System;
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

    /// <summary>
    /// 触发门
    /// </summary>
    /// <param name="waveIndex">怪物批次</param>
    public void TriggerMonsterBorn(TriggerData trigger, int waveIndex) {
        if (battleMgr != null) {
            // 将 trigger 变成 碰撞盒，将玩家锁在里面
            BoxCollider co = trigger.gameObject.GetComponent<BoxCollider>();
            co.isTrigger = false;
            // 触发怪物出生
            battleMgr.LoadMonsterByWaveID(waveIndex);
            battleMgr.ActiveCurrentBatchMonsters();
            battleMgr.triggerCheck = true;
        }
    }

    public TriggerData[] triggerArr;
    /// <summary>
    /// 通过一个小的关卡，触发下一个小关卡
    /// </summary>
    public bool SetNextTriggerOn() {
        // 找到对应的门，将其设为 trigger，可以通过
        waveIndex += 1;
        for (int i = 0; i < triggerArr.Length; i++) {
            if (triggerArr[i].triggerWave == waveIndex) {
                BoxCollider co = triggerArr[i].GetComponent<BoxCollider>();
                co.isTrigger = true;
                return true;
            }
        }
        return false; // 已经没有下一个 trigger 了，这已经是最后一个小关卡了
    }

}
