/****************************************************
    文件：PlayerController.cs
	作者：CaptainYun
    日期：2019/5/21 12:54:36
	功能：角色控制器
*****************************************************/

using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Transform camTrans; // 主摄像机位置
    private Vector3 camOffset; // 相机偏移
    public Animator ani;
    public CharacterController ctrl;
    private bool isMove = false; // 角色是否处于移动状态

    private Vector2 dir = Vector2.zero; // 当前角色的方向朝向
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

    private float targetBlend; // 目标 Blend值
    private float currentBlend; // 当前 Blend值


    public void Init() {
        camTrans = Camera.main.transform; // 获取主摄像机的位置
        camOffset = transform.position - camTrans.position; // 相机与玩家的偏移距离
    }

    private void Update() {
        #region Input 测试
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");
        //Vector2 _dir = new Vector2(h, v).normalized; // 方向
        //// 设置朝向
        //if (_dir != Vector2.zero) {
        //    Dir = _dir;
        //    SetBlend(Constants.BlendWalk);
        //}
        //else {
        //    Dir = Vector2.zero;
        //    SetBlend(Constants.BlendIdle);
        //}
        #endregion

        if (currentBlend != targetBlend) {
            UpdateMixBlend();
        }
        if (isMove) {
            // 设置方向
            SetDir();
            //产生移动
            SetMove();
            //相机跟随
            SetCam();
        }
    }

    /// <summary>
    /// 方向操作
    /// </summary>
    private void SetDir() {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1)) + camTrans.eulerAngles.y; // 朝向的偏移量 + 相机的偏移，否则相机显示出来会向相反的方向移动
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    /// <summary>
    /// 移动操作
    /// </summary>
    private void SetMove() {
        ctrl.Move(transform.forward * Time.deltaTime * Constants.PlayerMoveSpeed);
    }

    /// <summary>
    /// 相机跟随
    /// </summary>
    private void SetCam() {
        if (camTrans != null) {
            camTrans.position = transform.position - camOffset;
        }
    }

    /// <summary>
    /// 设置 Blend 值
    /// </summary>
    public void SetBlend(float blend) {
        targetBlend = blend;
    }

    /// <summary>
    /// 混合 Blend，平滑动画过渡
    /// </summary>
    private void UpdateMixBlend() {
        if (Mathf.Abs(currentBlend - targetBlend) < Constants.AccelerSpeed * Time.deltaTime) {
            currentBlend = targetBlend;
        }
        // 当前值比目标值大，逐渐减小
        else if (currentBlend > targetBlend) {
            currentBlend -= Constants.AccelerSpeed * Time.deltaTime;
        }
        // 当前值比目标值小，逐渐增大
        else {
            currentBlend += Constants.AccelerSpeed * Time.deltaTime;
        }
        ani.SetFloat("Blend", currentBlend);
    }

    
}