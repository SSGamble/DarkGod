/****************************************************
	文件：EntityPlayer.cs
	作者：CaptainYun
	日期：2019/06/02 13:02   	
	功能：玩家逻辑实体
*****************************************************/

using UnityEngine;

public class EntityPlayer : EntityBase {

    public override Vector2 GetDirInput() {
        return battleMgr.GetDirInput();
    }
}