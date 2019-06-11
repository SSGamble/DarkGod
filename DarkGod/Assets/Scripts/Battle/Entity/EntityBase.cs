/****************************************************
	文件：EntityBase.cs
	作者：CaptainYun
	日期：2019/06/02 13:01   	
	功能：角色逻辑实体 抽象基类：游戏实体数据变化
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase {
    // 管理器
    public StateMgr stateMgr = null;
    public BattleMgr battleMgr = null;
    public SkillMgr skillMgr = null;

    protected Controller controller = null; // 控制器
    public bool canControl = true; // 是否能控制角色
    public bool canRlsSkill = true; // 是否可以放技能
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

    public EntityType entityType = EntityType.None;
    public EntityState entityState = EntityState.None;

    // 实体名字
    private string name;
    public string Name {
        get {
            return name;
        }

        set {
            name = value;
        }
    }

    // 当前战斗中的血量变化
    private int hp;
    public int HP {
        get {
            return hp;
        }

        set {
            PECommon.Log("hp change:" + hp + "  to " + value);
            // 通知 UI 层
            SetHPVal(hp, value);
            hp = value;
        }
    }

    // 连招队列
    public Queue<int> comboQue = new Queue<int>();
    // 下一次的 攻击ID
    public int nextSkillID = 0;
    // 当前正在释放的技能的配置
    public SkillCfg curtSkillCfg;

    public virtual void SetBattleProps(BattleProps props) {
        HP = props.hp;
        Props = props;
    }

    // 技能位移的回调 ID
    public List<int> skMoveCBLst = new List<int>();
    // 技能伤害计算回调 ID
    public List<int> skActionCBLst = new List<int>();
    public int skEndCB = -1; // 技能结束时的回调

    #region 状态
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
    public void Die() {
        stateMgr.ChangeStatus(this, AniState.Die, null);
    }
    public void Hit() {
        stateMgr.ChangeStatus(this, AniState.Hit, null);
    }
    #endregion

    /// <summary>
    /// 处理 AI 逻辑
    /// </summary>
    public virtual void TickAILogic() {

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
        skillMgr.SkillAttack(this, skillID);
    }

    /// <summary>
    /// 攻击方向
    /// </summary>
    /// <param name="isCamoffset">是否需要计算摄像机偏移</param>
    public virtual void SetAtkRotation(Vector2 dir, bool isCamoffset = false) {
        if (controller != null) {
            if (isCamoffset) {
                controller.SetAtkRotationCam(dir);
            }
            else {
                controller.SetAtkRotationLocal(dir);
            }
        }
    }

    #region 战斗信息的显示
    public virtual void SetDodge() {
        if (controller != null) {
            GameRoot.Instance.dynamicWnd.SetDodge(Name);
        }
    }
    public virtual void SetCritical(int critical) {
        if (controller != null) {
            GameRoot.Instance.dynamicWnd.SetCritical(Name, critical);
        }
    }
    public virtual void SetHurt(int hurt) {
        if (controller != null) {
            GameRoot.Instance.dynamicWnd.SetHurt(Name, hurt);
        }
    }
    public virtual void SetHPVal(int oldval, int newval) {
        if (controller != null) {
            GameRoot.Instance.dynamicWnd.SetHPVal(Name, oldval, newval);
        }
    }
    #endregion

    public virtual Vector2 GetDirInput() {
        return Vector2.zero;
    }

    public virtual Vector3 GetPos() {
        return controller.transform.position;
    }

    public virtual Vector2 CalcTargetDir() {
        return Vector2.zero;
    }

    public virtual Transform GetTrans() {
        return controller.transform;
    }

    public AnimationClip[] GetAniClips() {
        if (controller != null) {
            return controller.ani.runtimeAnimatorController.animationClips;
        }
        return null;
    }

    public void SetController(Controller ctrl) {
        controller = ctrl;
    }

    public void SetActive(bool active = true) {
        if (controller != null) {
            controller.gameObject.SetActive(active);
        }
    }

    /// <summary>
    /// 退出攻击状态的时候调用
    /// </summary>
    public void ExitCurtSkill() {
        canControl = true;
        if (curtSkillCfg != null) {
            if (!curtSkillCfg.isBreak) {
                entityState = EntityState.None;
            }
            // 连招数据更新
            if (curtSkillCfg.isCombo) {
                if (comboQue.Count > 0) {
                    nextSkillID = comboQue.Dequeue();
                }
                else {
                    nextSkillID = 0;
                }
            }
            curtSkillCfg = null;
        }
        SetAction(Constants.ActionDefault);
    }

    public AudioSource GetAudio() {
        return controller.GetComponent<AudioSource>();
    }
    public CharacterController GetCC() {
        return controller.GetComponent<CharacterController>();
    }

    public virtual bool GetBreakState() {
        return true;
    }

    /// <summary>
    /// 移除 技能位移的回调 ID
    /// </summary>
    /// <param name="tid">待移除的 id </param>
    public void RmvMoveCB(int tid) {
        int index = -1;
        for (int i = 0; i < skMoveCBLst.Count; i++) {
            if (skMoveCBLst[i] == tid) {
                index = i;
                break;
            }
        }
        if (index != -1) {
            skMoveCBLst.RemoveAt(index);
        }
    }

    /// <summary>
    /// 移除 技能伤害的回调 ID
    /// </summary>
    /// <param name="tid">待移除的 id </param>
    public void RmvActionCB(int tid) {
        int index = -1;
        for (int i = 0; i < skActionCBLst.Count; i++) {
            if (skActionCBLst[i] == tid) {
                index = i;
                break;
            }
        }
        if (index != -1) {
            skActionCBLst.RemoveAt(index);
        }
    }

    /// <summary>
    /// 删除技能回调
    /// </summary>
    public void RmvSkillCB() {
        SetDir(Vector2.zero);
        SetSkillMoveState(false);

        // 删除定时回调任务
        for (int i = 0; i < skMoveCBLst.Count; i++) {
            int tid = skMoveCBLst[i];
            TimerSvc.Instance.DelTask(tid);
        }
        for (int i = 0; i < skActionCBLst.Count; i++) {
            int tid = skActionCBLst[i];
            TimerSvc.Instance.DelTask(tid);
        }

        // 攻击被中断，删除技能结束回到 idel 状态的定时回调
        if (skEndCB != -1) {
            TimerSvc.Instance.DelTask(skEndCB);
            skEndCB = -1;
        }
        skMoveCBLst.Clear();
        skActionCBLst.Clear();

        // 清空连招
        if (nextSkillID != 0 || comboQue.Count > 0) {
            nextSkillID = 0;
            comboQue.Clear();
            battleMgr.lastAtkTime = 0;
            battleMgr.comboIndex = 0;
        }
    }
}
