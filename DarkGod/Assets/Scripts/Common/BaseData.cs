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
    public int coin;
    public int exp;
    public int crystal;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;
    public List<MonsterData> monsterLst; // 怪物批次
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
    public int cdTime;
    public int skillTime; // 持续时间
    public int aniAction; // 动画机控制参数
    public string fx; // 特效资源
    public bool isCombo; // 是否是连招
    public bool isCollide; // 是否检测碰撞
    public bool isBreak; // 是否可以打断
    public DamageType dmgType; // 伤害类型
    public List<int> skillMoveLst; // 技能位移分阶段
    public List<int> skillActionLst; // 伤害点计算，eg：范围伤害，持续伤害
    public List<int> skillDamageLst; // 伤害
}

/// <summary>
/// 技能伤害点
/// </summary>
public class SkillActionCfg : BaseData<SkillActionCfg> {
    public int delayTime; // 延迟多少时间生效
    public float radius; // 伤害计算范围
    public int angle; // 伤害有效角度，以自己为圆心点
}

/// <summary>
/// 技能移动
/// </summary>
public class SkillMoveCfg : BaseData<SkillMoveCfg> {
    public int delayTime; // 延迟时间，延迟多久后开始移动
    public int moveTime; // 移动时间
    public float moveDis; // 移动距离
}

/// <summary>
/// 怪物
/// </summary>
public class MonsterCfg : BaseData<MonsterCfg> {
    public string mName;
    public MonsterType mType; // 1:普通怪物，2：boss 怪物
    public bool isStop; // 怪物是否能被攻击中断当前的状态
    public string resPath; // 在地图中的位置
    public int skillID; // 技能 id
    public float atkDis; // 攻击距离
    public BattleProps bps; // 战斗属性
}

/// <summary>
/// 怪物批次
/// </summary>
public class MonsterData : BaseData<MonsterData> {
    public int mWave; // 批次
    public int mIndex; // 序号，批次里面的第几个
    public MonsterCfg mCfg;
    public Vector3 mBornPos;
    public Vector3 mBornRote;
    public int mLevel; // 怪物等级
}

/// <summary>
/// 战斗属性数据
/// </summary>
public class BattleProps {
    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    public int dodge; // 闪避
    public int pierce; // 穿甲
    public int critical; // 暴击
}

