/****************************************************
	文件：Controller.cs
	作者：CaptainYun
	日期：2019/06/02 13:18   	
	功能：角色表现实体控制器 抽象基类：游戏场景物体表现
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour {

    protected Transform camTrans; // 主摄像机位置
    public Transform hpRoot; // 血条位置
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

    /// <summary>
    /// 攻击方向，不计算摄像机偏移
    /// </summary>
    public virtual void SetAtkRotationLocal(Vector2 atkDir) {
        float angle = Vector2.SignedAngle(atkDir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    /// <summary>
    /// 攻击方向，相机偏移
    /// </summary>
    public virtual void SetAtkRotationCam(Vector2 camDir) {
        float angle = Vector2.SignedAngle(camDir, new Vector2(0, 1)) + camTrans.eulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
}
