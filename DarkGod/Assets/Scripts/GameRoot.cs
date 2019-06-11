/****************************************************
   文件：GameRoot.cs
   作者：CaptainYun
   日期：2019/5/12 21:43:13
   功能：游戏的启动入口，初始化各个系统，保存核心数据
*****************************************************/

using PEProtocol;
using UnityEngine;

public class GameRoot : MonoBehaviour {

    // 单例
    public static GameRoot Instance = null;

    public LoadingWnd loadingWnd; // 加载进度界面
    public DynamicWnd dynamicWnd;

    private void Start() {
        Instance = this;
        DontDestroyOnLoad(this); // 当前 GameRoot 不需要销毁
        PECommon.Log("Game Start...");
        ClearUIRoot();
        Init();
    }

    /// <summary>
    /// 初始化 UI 窗口
    /// </summary>
    private void ClearUIRoot() {
        Transform canvas = transform.Find("Canvas");
        // 遍历 Canvas 下的所有子物体
        for (int i = 0; i < canvas.childCount; i++) {
            canvas.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 初始化各个业务系统和服务模块，应当先初始化服务后初始化业务
    /// </summary>
    private void Init() {

        // 服务模块初始化
        NetSvc net = GetComponent<NetSvc>();
        net.InitSvc();
        ResSvc res = GetComponent<ResSvc>();
        res.InitSvc();
        AudioSvc audio = GetComponent<AudioSvc>();
        audio.InitSvc();
        TimerSvc timer = GetComponent<TimerSvc>();
        timer.InitSvc();

        // 业务模块初始化
        LoginSys login = GetComponent<LoginSys>();
        login.InitSys();
        MainCitySys mainCity = GetComponent<MainCitySys>();
        mainCity.InitSys();
        DungeonSys dungeonSys = GetComponent<DungeonSys>();
        dungeonSys.InitSys();
        BattleSys battleSys = GetComponent<BattleSys>();
        battleSys.InitSys();

        // 显示动态窗口
        dynamicWnd.SetWndState();
        // 进入登录场景并加载相应 UI
        login.EnterLogin();
    }

    /// <summary>
    /// 添加一条 Tips
    /// </summary>
    /// <param name="tips">tips 内容</param>
    public static void AddTips(string tips) {
        Instance.dynamicWnd.EnqTips(tips);
    }

    // 玩家数据
    private PlayerData playerData = null;
    public PlayerData PlayerData {
        get {
            return playerData;
        }
    }

    #region 修改数据
    /// <summary>
    /// 设置 PlayerData
    /// </summary>
    public void SetPlayerData(RspLogin data) {
        playerData = data.playerData;
    }

    /// <summary>
    /// 更新玩家的名字 
    /// </summary>
    /// <param name="name"></param>
    public void SetPlayerName(string name) {
        PlayerData.name = name;
    }

    /// <summary>
    /// 完成任务后，修改数据
    /// </summary>
    public void SetPlayerDataByGuide(RspGuide data) {
        PlayerData.coin = data.coin;
        PlayerData.lv = data.lv;
        PlayerData.exp = data.exp;
        PlayerData.guideid = data.guideid;
    }

    /// <summary>
    /// 强化后，更新数据
    /// </summary>
    public void SetPlayerDataByStrong(RspStrong data) {
        PlayerData.coin = data.coin;
        PlayerData.crystal = data.crystal;
        PlayerData.hp = data.hp;
        PlayerData.ad = data.ad;
        PlayerData.ap = data.ap;
        PlayerData.addef = data.addef;
        PlayerData.apdef = data.apdef;
        PlayerData.strongArr = data.strongArr;
    }

    /// <summary>
    /// 购买后，修改数据
    /// </summary>
    public void SetPlayerDataByBuy(RspBuy data) {
        PlayerData.diamond = data.dimond;
        PlayerData.coin = data.coin;
        PlayerData.power = data.power;
    }

    /// <summary>
    /// 设置玩家体力值
    /// </summary>
    public void SetPlayerDataByPower(PshPower data) {
        PlayerData.power = data.power;
    }

    /// <summary>
    /// 领取任务奖励后
    /// </summary>
    public void SetPlayerDataByTask(RspTakeTaskReward data) {
        PlayerData.coin = data.coin;
        PlayerData.lv = data.lv;
        PlayerData.exp = data.exp;
        PlayerData.taskArr = data.taskArr;
    }

    /// <summary>
    /// 更改任务进度
    /// </summary>
    /// <param name="data"></param>
    public void SetPlayerDataByTaskPsh(PshTaskPrgs data) {
        PlayerData.taskArr = data.taskArr;
    }

    /// <summary>
    /// 进入副本后的体力变化
    /// </summary>
    /// <param name="data"></param>
    public void SetPlayerDataByDungeonStart(RspDungeonFight data) {
        PlayerData.power = data.power;
    }

    /// <summary>
    /// 副本结束后奖励
    /// </summary>
    /// <param name="data"></param>
    public void SetPlayerDataByDungeonEnd(RspDungeonFightEnd data) {
        PlayerData.coin = data.coin;
        PlayerData.lv = data.lv;
        PlayerData.exp = data.exp;
        PlayerData.crystal = data.crystal;
        PlayerData.dungeon = data.dungeon;
    }
    #endregion

}