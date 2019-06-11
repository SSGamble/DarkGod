/****************************************************
    文件：TriggerData.cs
	作者：CaptainYun
    日期：2019/6/10 10:3:47
	功能：地图触发数据类
*****************************************************/

using UnityEngine;

public class TriggerData : MonoBehaviour {

    public int triggerWave; // 触发哪一批次的怪物
    public MapMgr mapMgr;

    public void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            if (mapMgr != null) {
                mapMgr.TriggerMonsterBorn(this, triggerWave);
            }
        }
    }
}