/****************************************************
	文件：NewBehaviourScript1.cs
	作者：CaptainYun
	日期：2019/05/14 16:53   	
	功能：路径常量
*****************************************************/
using UnityEngine;
using System.Collections;

public class PathDefine {

    #region 配置文件
    public const string RanNameCfg = "ResCfgs/rdname"; // 随机名字
    public const string MapCfg = "ResCfgs/map";  // 地图
    public const string GuideCfg = "ResCfgs/guide"; // 自动引导
    #endregion

    #region 自动任务引导
    public const string TaskHead = "ResImages/task";
    public const string WiseManHead = "ResImages/wiseman";
    public const string GeneralHead = "ResImages/general";
    public const string ArtisanHead = "ResImages/artisan";
    public const string TraderHead = "ResImages/trader";

    public const string SelfIcon = "ResImages/assassin";
    public const string GuideIcon = "ResImages/npcguide";
    public const string WiseManIcon = "ResImages/npc0";
    public const string GeneralIcon = "ResImages/npc1";
    public const string ArtisanIcon = "ResImages/npc2";
    public const string TraderIcon = "ResImages/npc3";
    #endregion

    #region Player
    public const string AssissnCityPlayerPrefab = "PrefabPlayer/AssassinCity";
    #endregion

    #region 强化
    public const string ItemArrorBG = "ResImages/btnstrong";
    public const string ItemPlatBG = "ResImages/charbg3";

    public const string ItemToukui = "ResImages/toukui";
    public const string ItemBody = "ResImages/body";
    public const string ItemYaobu = "ResImages/yaobu";
    public const string ItemHand = "ResImages/hand";
    public const string ItemLeg = "ResImages/leg";
    public const string ItemFoot = "ResImages/foot";

    public const string SpStar1 = "ResImages/star1";
    public const string SpStar2 = "ResImages/star2";
    #endregion
}
