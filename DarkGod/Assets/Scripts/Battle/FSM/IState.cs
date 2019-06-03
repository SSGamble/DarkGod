/****************************************************
	文件：IState.cs
	作者：CaptainYun
	日期：2019/06/02 12:59   	
	功能：状态接口
*****************************************************/

public interface IState {
    // 进入状态，entity：谁进入的，args: 技能管理器注入与可变传参
    void Enter(EntityBase entity, params object[] args);
    // 处理状态，在这个状态下要做什么样的事情
    void Process(EntityBase entity, params object[] args);
    // 离开状态
    void Exit(EntityBase entity, params object[] args);
}

/// <summary>
/// 状态枚举
/// </summary>
public enum AniState {
    None,
    Idle,
    Move,
    Attack,
    Born,
}
