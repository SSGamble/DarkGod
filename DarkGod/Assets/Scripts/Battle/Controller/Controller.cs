/****************************************************
	文件：Controller.cs
	作者：CaptainYun
	日期：2019/06/02 13:18   	
	功能：角色表现实体控制器 抽象基类：游戏场景物体表现
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour {
    public CharacterController ctrl;
    public Animator ani;
    protected bool isMove = false;
    private Vector2 dir = Vector2.zero;
    public Vector2 Dir {
        get {
            return dir;
        }

        set {
            if (value == Vector2.zero) {
                isMove = false;
            }
            else {
                isMove = true;
            }
            dir = value;
        }
    }

    protected bool skillMove = false; // 技能移动
    protected float skillMoveSpeed = 0f;

    protected TimerSvc timerSvc;
    // 特效字典
    protected Dictionary<string, GameObject> fxDic = new Dictionary<string, GameObject>();

    public virtual void Init() {
        timerSvc = TimerSvc.Instance;
    }

    public virtual void SetBlend(float blend) {
        ani.SetFloat("Blend", blend);
    }

    public virtual void SetAction(int act) {
        ani.SetInteger("Action", act);
    }

    public virtual void SetFX(string name, float destroy) {

    }

    public void SetSkillMoveState(bool move, float skillSpeed = 0f) {
        skillMove = move;
        skillMoveSpeed = skillSpeed;
    }
}
