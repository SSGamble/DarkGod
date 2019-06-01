/****************************************************
	文件：BaseData.cs
	作者：CaptainYun
	日期：2019/05/21 17:40   	
	功能：配置数据类
*****************************************************/

using UnityEngine;

/// <summary>
/// 基类
/// </summary>
public class BaseData<T> {
    public int id;
}

/// <summary>
/// 地图
/// </summary>
public class MapCfg : BaseData<MapCfg> {
    public string mapName;
    public string sceneName;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;
}

/// <summary>
/// 自动引导
/// </summary>
public class AutoGuideCfg : BaseData<AutoGuideCfg> {
    public int npcID; // 触发任务目标 NPC 索引号
    public string dilogArr; // 对话数据
    public int actID; // 目标任务 ID
    public int coin; // 金币奖励
    public int exp; // 经验奖励
}

/// <summary>
/// 强化升级
/// </summary>
public class StrongCfg : BaseData<StrongCfg> {
    public int pos; // 位置，在 UI 界面，左侧分类列表的位置
    public int startlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int coin;
    public int crystal;
}

/// <summary>
/// 任务奖励
/// </summary>
public class TaskRewardCfg : BaseData<TaskRewardCfg> {
    public string taskName;
    public int count; // 任务计数
    public int exp;
    public int coin;
}

/// <summary>
/// 任务进度
/// </summary>
public class TaskRewardData : BaseData<TaskRewardData> {
    public int prgs; // 进度
    public bool taked; // 是否已经被领取奖励
}


