/****************************************************
	文件：StateMgr.cs
	作者：CaptainYun
	日期：2019/06/02 10:18   	
	功能：状态管理器
*****************************************************/

using UnityEngine;
using System.Collections.Generic;

public class StateMgr : MonoBehaviour {

    private Dictionary<AniState, IState> fsm = new Dictionary<AniState, IState>();

    public void Init() {
        fsm.Add(AniState.Idle, new StateIdle());
        fsm.Add(AniState.Move, new StateMove());
        fsm.Add(AniState.Attack, new StateAttack());

        PECommon.Log("Init StateMgr Done.");
    }

    /// <summary>
    /// 状态切换
    /// </summary>
    /// <param name="entity">谁改变状态</param>
    /// <param name="targetState">目标状态</param>
    public void ChangeStatus(EntityBase entity, AniState targetState, params object[] args) {
        if (entity.currentAniState == targetState) {
            return;
        }

        if (fsm.ContainsKey(targetState)) {
            if (entity.currentAniState != AniState.None) {
                fsm[entity.currentAniState].Exit(entity, args); // 退出当前状态
            }
            fsm[targetState].Enter(entity, args); // 进入目标状态
            fsm[targetState].Process(entity, args);
        }
    }
}

