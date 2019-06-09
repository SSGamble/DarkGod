/****************************************************
	文件：EntityPlayer.cs
	作者：CaptainYun
	日期：2019/06/02 13:02   	
	功能：玩家逻辑实体
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : EntityBase {

    public EntityPlayer() {
        entityType = EntityType.Player;
    }

    public override Vector2 GetDirInput() {
        return battleMgr.GetDirInput();
    }

    /// <summary>
    /// 计算目标的方向
    /// </summary>
    public override Vector2 CalcTargetDir() {
        EntityMonster monster = FindClosedTarget();
        if (monster != null) {
            Vector3 target = monster.GetPos();
            Vector3 self = GetPos();
            Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
            return dir.normalized;
        }
        else {
            return Vector2.zero;
        }
    }

    /// <summary>
    /// 寻找距离玩家最近的怪物
    /// </summary>
    private EntityMonster FindClosedTarget() {
        // 查找出场景所有的怪物
        List<EntityMonster> lst = battleMgr.GetEntityMonsters();
        if (lst == null || lst.Count == 0) {
            return null;
        }
        // 遍历计算距离
        Vector3 self = GetPos();
        EntityMonster targetMonster = null;
        float dis = 0;
        for (int i = 0; i < lst.Count; i++) {
            Vector3 target = lst[i].GetPos();
            if (i == 0) {
                dis = Vector3.Distance(self, target); // 距离
                targetMonster = lst[0];
            }
            else {
                float calcDis = Vector3.Distance(self, target);
                // 保留最小记录的怪物 
                if (dis > calcDis) {
                    dis = calcDis;
                    targetMonster = lst[i];
                }
            }
        }
        return targetMonster;
    }

    public override void SetHPVal(int oldval, int newval) {
        BattleSys.Instance.playerCtrlWnd.SetSelfHPBarVal(newval);
    }

    public override void SetDodge() {
        GameRoot.Instance.dynamicWnd.SetSelfDodge();
    }
}