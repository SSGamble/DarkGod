/****************************************************
	文件：StateMove.cs
	作者：CaptainYun
	日期：2019/06/02 13:04   	
	功能：移动状态
*****************************************************/

public class StateMove : IState {

    public void Enter(EntityBase entity, params object[] args) {
        entity.currentAniState = AniState.Move;
        PECommon.Log("Enter StateMove.");
    }

    public void Exit(EntityBase entity, params object[] args) {
        PECommon.Log("Exit StateMove.");
    }

    public void Process(EntityBase entity, params object[] args) {
        PECommon.Log("Process StateMove.");
        entity.SetBlend(Constants.BlendMove);
    }
}