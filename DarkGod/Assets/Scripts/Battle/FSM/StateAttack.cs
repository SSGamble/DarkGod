/****************************************************
	文件：StateAttack.cs
	作者：CaptainYun
	日期：2019/06/02 16:03   	
	功能：攻击状态
*****************************************************/

public class StateAttack : IState {

    public void Enter(EntityBase entity, params object[] args) {
        entity.currentAniState = AniState.Attack;
        PECommon.Log("Enter StateAttack.");
    }

    public void Exit(EntityBase entity, params object[] args) {
        entity.canControl = true;
        entity.SetAction(Constants.ActionDefault);
        PECommon.Log("Exit StateAttack.");
    }


    public void Process(EntityBase entity, params object[] args) {
        entity.AttackEffect((int)args[0]);
        PECommon.Log("Process StateAttack.");
        //entity.SetAction(1);
    }
}