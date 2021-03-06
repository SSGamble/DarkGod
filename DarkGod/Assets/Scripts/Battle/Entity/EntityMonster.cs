/****************************************************
	文件：EntityMonster.cs
	作者：CaptainYun
	日期：2019/06/03 10:14   	
	功能：怪物逻辑实体
*****************************************************/

using UnityEngine;

public class EntityMonster : EntityBase {

    public MonsterData md;

    private float checkTime = 2; // 检测时间间隔
    private float checkCountTime = 0; // 检测计时
    private float atkTime = 2;
    private float atkCountTime = 0;
    private bool runAI = true; // 是否运行 AI

    public EntityMonster() {
        entityType = EntityType.Monster;
    }

    /// <summary>
    /// 等级影响
    /// </summary>
    /// <param name="props"></param>
    public override void SetBattleProps(BattleProps props) {
        int level = md.mLevel;
        BattleProps p = new BattleProps {
            hp = props.hp * level,
            ad = props.ad * level,
            ap = props.ap * level,
            addef = props.addef * level,
            apdef = props.apdef * level,
            dodge = props.dodge * level,
            pierce = props.pierce * level,
            critical = props.critical * level
        };
        Props = p;
        HP = p.hp;
    }

    /// <summary>
    /// 怪物的 AI 逻辑
    /// </summary>
    public override void TickAILogic() {
        if (!runAI) {
            return;
        }
        // 防止还在 出生状态 就开始移动，从而出现滑动效果
        if (currentAniState == AniState.Idle || currentAniState == AniState.Move) {
            // 暂停游戏
            if (battleMgr.isPauseGame) {
                Idle();
                return;
            }
            // 检测时间间隔计算
            float delta = Time.deltaTime;
            checkCountTime += delta;
            if (checkCountTime < checkTime) {
                return;
            }
            else {
                // 计算目标方向
                Vector2 dir = CalcTargetDir();
                // 判断目标是否在攻击范围
                if (!InAtkRange()) { // 不在，设置移动方向，移动
                    SetDir(dir);
                    Move();
                }
                else { // 在，停止移动，攻击
                    SetDir(Vector2.zero);
                    // 判断攻击间隔
                    atkCountTime += checkCountTime; // 把移动的时间当成时间间隔的一部分，防止移动到目标身边后还要等待一会才会攻击
                    if (atkCountTime > atkTime) { // 达到攻击时间，转向并攻击
                        SetAtkRotation(dir);
                        Attack(md.mCfg.skillID);
                        atkCountTime = 0;
                    }
                    else { // 等待
                        Idle();
                    }
                }
                checkCountTime = 0;
                checkTime = PETools.RanInt(1, 5) * 1.0f / 10; // 第一次 2 s，之后随机检测时间间隔
            }
        }
    }

    /// <summary>
    /// 计算目标(玩家)方向
    /// </summary>
    /// <returns></returns>
    public override Vector2 CalcTargetDir() {
        EntityPlayer entityPlayer = battleMgr.entitySelfPlayer;
        if (entityPlayer == null || entityPlayer.currentAniState == AniState.Die) {
            runAI = false;
            return Vector2.zero;
        }
        else {
            Vector3 target = entityPlayer.GetPos();
            Vector3 self = GetPos();
            return new Vector2(target.x - self.x, target.z - self.z).normalized;
        }
    }

    /// <summary>
    /// 判断目标是否在攻击范围
    /// </summary>
    /// <returns></returns>
    private bool InAtkRange() {
        EntityPlayer entityPlayer = battleMgr.entitySelfPlayer;
        if (entityPlayer == null || entityPlayer.currentAniState == AniState.Die) {
            runAI = false;
            return false;
        }
        else {
            Vector3 target = entityPlayer.GetPos();
            Vector3 self = GetPos();
            target.y = 0;
            self.y = 0;
            float dis = Vector3.Distance(target, self);
            if (dis <= md.mCfg.atkDis) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    /// <summary>
    /// 是否能被中断
    /// </summary>
    /// <returns></returns>
    public override bool GetBreakState() {
        if (md.mCfg.isStop) { // isStop 全局的
            if (curtSkillCfg != null) {
                return curtSkillCfg.isBreak; // isBreak 局部的，针对技能
            }
            else {
                return true;
            }
        }
        else {
            return false;
        }
    }

    /// <summary>
    /// 更新血条
    /// </summary>
    public override void SetHPVal(int oldVal, int newVal) {
        if (md.mCfg.mType == MonsterType.Boss) {
            BattleSys.Instance.playerCtrlWnd.SetBossHPBarVal(oldVal, newVal, Props.hp);
        }
        else {
            base.SetHPVal(oldVal, newVal);
        }
    }
}