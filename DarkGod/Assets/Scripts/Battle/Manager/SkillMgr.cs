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
    /// 技能效果表现
    /// </summary>
    public void AttackEffect(EntityBase entity, int skillID) {
        SkillCfg skillData = resSvc.GetSkillCfg(skillID);
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