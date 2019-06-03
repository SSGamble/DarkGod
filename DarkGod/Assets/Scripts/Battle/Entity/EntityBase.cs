/****************************************************
	文件：EntityBase.cs
	作者：CaptainYun
	日期：2019/06/02 13:01   	
	功能：角色逻辑实体 抽象基类：游戏实体数据变化
*****************************************************/

using System;
using UnityEngine;

public abstract class EntityBase {
    // 管理器
    public StateMgr stateMgr = null; 
    public BattleMgr battleMgr = null;
    public SkillMgr skillMgr = null;

    public Controller controller = null; // 控制器
    public bool canControl = true; // 是否能控制角色
    public AniState currentAniState = AniState.None; // 默认状态

    private BattleProps props;  // 战斗属性
    public BattleProps Props {
        get {
            return props;
        }

        protected set {
            props = value;
        }
    }

    // 当前战斗中的血量变化
    private int hp;
    public int HP {
        get {
            return hp;
        }

        set {
            //通知 UI 层 TODO
            PECommon.Log("hp change:" + hp + "  to " + value);
            hp = value;
        }
    }

    public virtual void SetBattleProps(BattleProps props) {
        HP = props.hp;
        Props = props;
    }

    public void Born() {
        stateMgr.ChangeStatus(this, AniState.Born, null);
    }
    public void Move() {
        stateMgr.ChangeStatus(this, AniState.Move, null);
    }
    public void Idle() {
        stateMgr.ChangeStatus(this, AniState.Idle, null);
    }
    public void Attack(int skillID) {
        stateMgr.ChangeStatus(this, AniState.Attack, skillID);
    }

    /// <summary>
    /// 设置 Blend 值
    /// </summary>
    public virtual void SetBlend(float blend) {
        if (controller != null) {
            controller.SetBlend(blend);
        }
    }

    /// <summary>
    /// 设置方向
    /// </summary>
    public virtual void SetDir(Vector2 dir) {
        if (controller != null) {
            controller.Dir = dir;
        }
    }

    /// <summary>
    /// 动作，状态动画机，参数
    /// </summary>
    /// <param name="act"></param>
    public virtual void SetAction(int act) {
        if (controller != null) {
            controller.SetAction(act);
        }
    }

    /// <summary>
    /// 设置特效
    /// </summary>
    /// <param name="name">特效名字</param>
    /// <param name="destroy">播放多长时间</param>
    public virtual void SetFX(string name, float destroy) {
        if (controller != null) {
            controller.SetFX(name, destroy);
        }
    }

    /// <summary>
    /// 技能移动
    /// </summary>
    public virtual void SetSkillMoveState(bool move, float speed = 0f) {
        if (controller != null) {
            controller.SetSkillMoveState(move, speed);
        }
    }

    /// <summary>
    /// 技能攻击
    /// </summary>
    /// <param name="skillID"></param>
    public virtual void SkillAttack(int skillID) {
        skillMgr.AttackEffect(this, skillID);
    }

    /// <summary>
    /// 攻击伤害
    /// </summary>
    //public virtual void AttackDamage(int skillID) {
    //    skillMgr.AttackDamage(this, skillID);
    //}

    public virtual Vector2 GetDirInput() {
        return Vector2.zero;
    }

    public virtual Vector3 GetPos() {
        return controller.transform.position;
    }

    public virtual Transform GetTrans() {
        return controller.transform;
    }
}
