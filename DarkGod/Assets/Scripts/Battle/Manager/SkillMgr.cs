/****************************************************
	文件：SkillMgr.cs
	作者：CaptainYun
	日期：2019/06/02 10:19   	
	功能：技能管理器
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillMgr : MonoBehaviour {

    private ResSvc resSvc;
    private TimerSvc timeSvc;

    public void Init() {
        resSvc = ResSvc.Instance;
        timeSvc = TimerSvc.Instance;
        PECommon.Log("Init SkillMgr Done.");
    }

    /// <summary>
    /// 技能攻击
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="skillID"></param>
    public void SkillAttack(EntityBase entity, int skillID) {
        AttackDamage(entity, skillID);
        AttackEffect(entity, skillID);
    }

    /// <summary>
    /// 技能效果表现
    /// </summary>
    public void AttackEffect(EntityBase entity, int skillID) {
        SkillCfg skillData = resSvc.GetSkillCfg(skillID);
        // 没有方向输入
        if (entity.GetDirInput() == Vector2.zero) {
            // 搜索最近的怪物
            Vector2 dir = entity.CalcTargetDir();
            if (dir != Vector2.zero) {
                entity.SetAtkRotation(dir);
            }
        }
        // 有方向输入
        else {
            entity.SetAtkRotation(entity.GetDirInput(), true);
        }
        entity.SetAction(skillData.aniAction);
        entity.SetFX(skillData.fx, skillData.skillTime);

        CalcSkillMove(entity, skillData);
        entity.canControl = false; // 释放技能的时候，禁止操控角色
        entity.SetDir(Vector2.zero); // 防止移动对技能产生影响

        // 回到 Idel 状态
        timeSvc.AddTimeTask((int tid) => {
            entity.Idle();
        }, skillData.skillTime);
    }

    /// <summary>
    /// 技能伤害数值
    /// </summary>
    public void AttackDamage(EntityBase entity, int skillID) {
        SkillCfg skillData = resSvc.GetSkillCfg(skillID);
        List<int> actonLst = skillData.skillActionLst;
        int sum = 0; // 总伤害
        for (int i = 0; i < actonLst.Count; i++) {
            SkillActionCfg skillAction = resSvc.GetSkillActionCfg(actonLst[i]);
            sum += skillAction.delayTime;
            int index = i;
            if (sum > 0) {
                timeSvc.AddTimeTask((int tid) => {
                    SkillAction(entity, skillData, index);
                }, sum);
            }
            else { // 瞬时技能，不需要延迟
                SkillAction(entity, skillData, index);
            }
        }
    }

    /// <summary>
    /// 技能伤害计算
    /// </summary>
    public void SkillAction(EntityBase caster, SkillCfg skillCfg, int index) {
        SkillActionCfg skillActionCfg = resSvc.GetSkillActionCfg(skillCfg.skillActionLst[index]);
        int damage = skillCfg.skillDamageLst[index];
        // 获取场景里所有的怪物实体，遍历运算，怪物很多的游戏 如 MMO 就不能这样运算
        List<EntityMonster> monsterLst = caster.battleMgr.GetEntityMonsters();
        for (int i = 0; i < monsterLst.Count; i++) {
            EntityMonster target = monsterLst[i];
            // 判断距离，判断角度
            if (InRange(caster.GetPos(), target.GetPos(), skillActionCfg.radius)
                && InAngle(caster.GetTrans(), target.GetPos(), skillActionCfg.angle)) {
                // 计算伤害
                CalcDamage(caster, target, skillCfg, damage);
            }
        }
    }

    /// <summary>
    /// 是否在指定范围里面
    /// </summary>
    private bool InRange(Vector3 from, Vector3 to, float range) {
        float dis = Vector3.Distance(from, to);
        if (dis <= range) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 是否在指定角度里面
    /// </summary>
    private bool InAngle(Transform trans, Vector3 to, float angle) {
        if (angle == 360) {
            return true;
        }
        else {
            Vector3 start = trans.forward; // 人物正朝向
            Vector3 dir = (to - trans.position).normalized; // 目标朝向
            float ang = Vector3.Angle(start, dir); // 夹角
            if (ang <= angle / 2) {
                return true;
            }
            return false;
        }
    }

    System.Random rd = new System.Random();

    /// <summary>
    /// 伤害运算
    /// </summary>
    private void CalcDamage(EntityBase caster, EntityBase target, SkillCfg skillCfg, int damage) {
        int dmgSum = damage;
        // AD 伤害
        if (skillCfg.dmgType == DamageType.AD) {
            // 计算闪避
            int dodgeNum = PETools.RanInt(1, 100, rd);
            if (dodgeNum <= target.Props.dodge) {
                // UI 显示闪避 TODO
                PECommon.Log("闪避Rate:" + dodgeNum + "/" + target.Props.dodge);
                target.SetDodge();
                return;
            }
            // 计算属性加成
            dmgSum += caster.Props.ad;
            // 计算暴击
            int criticalNum = PETools.RanInt(1, 100, rd);
            if (criticalNum <= caster.Props.critical) {
                float criticalRate = 1 + (PETools.RanInt(1, 100, rd) / 100.0f); // 暴击率
                dmgSum = (int)criticalRate * dmgSum;
                PECommon.Log("暴击Rate:" + criticalNum + "/" + caster.Props.critical);
                target.SetCritical(dmgSum);
            }
            // 计算穿甲 
            int addef = (int)((1 - caster.Props.pierce / 100.0f) * target.Props.addef);
            dmgSum -= addef;
        }
        // AP 伤害
        else if (skillCfg.dmgType == DamageType.AP) {
            // 计算属性加成
            dmgSum += caster.Props.ap;
            // 计算魔法抗性
            dmgSum -= target.Props.apdef;
        }
        else { }

        // 最终伤害
        if (dmgSum < 0) {
            dmgSum = 0;
            return;
        }

        target.SetHurt(dmgSum);

        if (target.HP < dmgSum) {
            target.HP = 0;
            target.Die(); // 目标死亡
            target.battleMgr.RmvMonster(target.Name); // 移除
        }
        else {
            target.HP -= dmgSum;
            target.Hit();
        }
    }

    /// <summary>
    /// 计算技能移动
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="skillData"></param>
    private void CalcSkillMove(EntityBase entity, SkillCfg skillData) {
        List<int> skillMoveLst = skillData.skillMoveLst;
        int sum = 0;
        // 遍历所有的技能位移，一个技能可能会有多段位移
        for (int i = 0; i < skillMoveLst.Count; i++) {
            SkillMoveCfg skillMoveCfg = resSvc.GetSkillMoveCfg(skillData.skillMoveLst[i]);
            float speed = skillMoveCfg.moveDis / (skillMoveCfg.moveTime / 1000f);
            sum += skillMoveCfg.delayTime; // 总的延迟时间
            if (sum > 0) {
                timeSvc.AddTimeTask((int tid) => {
                    entity.SetSkillMoveState(true, speed);
                }, sum);
            }
            else {
                entity.SetSkillMoveState(true, speed);
            }

            sum += skillMoveCfg.moveTime;
            timeSvc.AddTimeTask((int tid) => {
                entity.SetSkillMoveState(false);
            }, sum);
        }
    }
}