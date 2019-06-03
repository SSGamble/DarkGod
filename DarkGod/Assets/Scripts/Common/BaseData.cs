/****************************************************
	文件：BaseData.cs
	作者：CaptainYun
	日期：2019/05/21 17:40   	
	功能：配置数据类
*****************************************************/

using System.Collections.Generic;
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
    public int power;
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

/// <summary>
/// 技能
/// </summary>
public class SkillCfg : BaseData<SkillCfg> {
    public string skillName;
    public int skillTime; // 持续时间
    public int aniAction; // 动画机控制参数
    public string fx;
    public List<int> skillMoveLst; // 技能位移分阶段
}

/// <summary>
/// 技能移动
/// </summary>
public class SkillMoveCfg : BaseData<SkillMoveCfg> {
    public int delayTime; // 延迟时间，延迟多久后开始移动
    public int moveTime; // 移动时间
    public float moveDis; // 移动距离
}

