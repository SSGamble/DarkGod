/****************************************************
	文件：StateIdle.cs
	作者：CaptainYun
	日期：2019/06/02 13:04   	
	功能：待机状态
*****************************************************/

using UnityEngine;

public class StateIdle : IState {

    public void Enter(EntityBase entity, params object[] args) {
        entity.currentAniState = AniState.Idle;
        entity.SetDir(Vector2.zero);
        //PECommon.Log("Enter StateIdle.");
    }

    public void Exit(EntityBase entity, params object[] args) {
        //PECommon.Log("Exit StateIdle.");
    }

    public void Process(EntityBase entity, params object[] args) {
        if (entity.nextSkillID != 0) { // 开始连招
            entity.Attack(entity.nextSkillID);
        }
        else {
            // 轮盘一直处于移动状态时，释放了技能后，尽管没有动轮盘，但依然需要他按照之前的方向移动
            if (entity.GetDirInput() != Vector2.zero) {
                entity.Move();
                entity.SetDir(entity.GetDirInput());
            }
            else { // 怪物，AI 驱动
                entity.SetBlend(Constants.BlendIdle);
            }
        }
        //PECommon.Log("Process StateIdle.");
    }
}