/****************************************************
    文件：MainCitySvc.cs
	作者：CaptainYun
    日期：2019/5/20 14:6:35
	功能：主城业务系统
*****************************************************/

using System;
using PEProtocol;
using UnityEngine;
using UnityEngine.AI;

public class MainCitySys : SystemRoot {
    // 单例
    public static MainCitySys Instance = null;

    public MainCityWnd mainCityWnd;
    public InfoWnd infoWnd;
    public GuideWnd guideWnd;
    public StrongWnd strongWnd;
    public ChatWnd chatWnd;
    public BuyWnd buyWnd;

    private Transform charCamTrans; // 拍摄主角的相机位置

    public override void InitSys() {
        base.InitSys();
        Instance = this;
        PECommon.Log("Init MainCitySvc..");
    }

    /// <summary>
    /// 进入主城
    /// </summary>
    public void EnterMainCity() {
        MapCfg mapData = resSvc.GetMapCfgData(Constants.MainCityMapID);
        resSvc.AsyncLoadScene(mapData.sceneName, () => {
            // 加载主角
            LoadPlayer(mapData);
            // 打开主城界面
            mainCityWnd.SetWndState(true);
            // 播放主城背景音乐
            audioSvc.PlayBGAudio(Constants.BGMainCity);
            // 获取主城地图中的 npc 位置
            GameObject mapRoot = GameObject.FindGameObjectWithTag("MapRoot");
            MainCityMap map = mapRoot.GetComponent<MainCityMap>();
            npcPosTrans = map.npcPosTrans;
            // 设置角色显示相机
            if (charCamTrans != null) {
                charCamTrans.gameObject.SetActive(false);
            }
        });
    }

    #region 角色控制

    private PlayerController playerCtrl;

    /// <summary>
    /// 加载角色预制体
    /// </summary>
    /// <param name="mapCfg"></param>
    private void LoadPlayer(MapCfg mapData) {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssissnCityPlayerPrefab, true);
        player.transform.position = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        // 相机初始化
        Camera.main.transform.position = mapData.mainCamPos;
        Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.Init();

        nav = player.GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// 控制角色移动
    /// </summary>
    /// <param name="dir"></param>
    public void SetMoveDir(Vector2 dir) {
        StopNavTask();
        // 动画
        if (dir == Vector2.zero) {
            playerCtrl.SetBlend(Constants.BlendIdle);
        }
        else {
            playerCtrl.SetBlend(Constants.BlendWalk);
        }
        // 方向
        playerCtrl.Dir = dir;
    }
    #endregion

    #region 角色信息窗口

    /// <summary>
    /// 打开角色信息窗口
    /// </summary>
    public void OpenInfoWnd() {
        StopNavTask();
        if (charCamTrans == null) {
            charCamTrans = GameObject.FindGameObjectWithTag("CharShowCam").transform;
        }
        // 设置人物展示相机相对位置
        charCamTrans.localPosition = playerCtrl.transform.position + playerCtrl.transform.forward * 2.3f + new Vector3(0, 1.2f, 0);
        charCamTrans.localEulerAngles = new Vector3(0, 180 + playerCtrl.transform.localEulerAngles.y, 0);
        charCamTrans.localScale = Vector3.one;
        charCamTrans.gameObject.SetActive(true);
        // 显示窗口
        infoWnd.SetWndState();
    }

    /// <summary>
    /// 关闭角色信息窗口
    /// </summary>
    public void CloseInfoWnd() {
        if (charCamTrans != null) {
            charCamTrans.gameObject.SetActive(false); // 关闭角色相机
            infoWnd.SetWndState(false);
        }
    }

    /// <summary>
    /// 默认开始旋转角度
    /// </summary>
    private float startRoate = 0;

    /// <summary>
    /// 设置角色开始的旋转角度
    /// </summary>
    public void SetStartRoate() {
        startRoate = playerCtrl.transform.localEulerAngles.y;
    }

    /// <summary>
    /// 设置角色的旋转
    /// </summary>
    public void SetPlayerRoate(float roate) {
        playerCtrl.transform.localEulerAngles = new Vector3(0, startRoate + roate, 0);
    }
    #endregion

    #region 任务引导
    private AutoGuideCfg curtTaskData; // 任务数据
    private Transform[] npcPosTrans;
    private NavMeshAgent nav; // 导航组件 
    private bool isNavGuide = false; // 是否在导航

    public AutoGuideCfg GetCurtTaskData() {
        return curtTaskData;
    }

    /// <summary>
    /// 执行任务
    /// </summary>
    /// <param name="agc"></param>
    public void RunTask(AutoGuideCfg agc) {
        if (agc != null) {
            curtTaskData = agc;
        }

        //解析任务数据
        nav.enabled = true;
        if (curtTaskData.npcID != -1) {
            float dis = Vector3.Distance(playerCtrl.transform.position, npcPosTrans[agc.npcID].position); // 当前主角和目标 npc 的距离
            if (dis < 0.5f) { // 已找到目标 npc ，停止导航
                isNavGuide = false;
                nav.isStopped = true;
                playerCtrl.SetBlend(Constants.BlendIdle); //动画
                nav.enabled = false;

                OpenGuideWnd();
            }
            else {
                isNavGuide = true;
                nav.enabled = true;
                nav.speed = Constants.PlayerMoveSpeed;
                nav.SetDestination(npcPosTrans[agc.npcID].position); // 目标 npc 的位置
                playerCtrl.SetBlend(Constants.BlendWalk); // 动画
            }
        }
        else {
            OpenGuideWnd();
        }
    }

    private void Update() {
        // 自动导航的时候，更新相机的移动
        if (isNavGuide) {
            IsArriveNavPos();
            playerCtrl.SetCam(); // 相机跟随
        }
    }

    /// <summary>
    /// 是否到达目标位置
    /// </summary>
    private void IsArriveNavPos() {
        float dis = Vector3.Distance(playerCtrl.transform.position, npcPosTrans[curtTaskData.npcID].position);
        if (dis < 0.5f) {
            isNavGuide = false;
            nav.isStopped = true;
            playerCtrl.SetBlend(Constants.BlendIdle);
            nav.enabled = false;

            OpenGuideWnd();
        }
    }

    /// <summary>
    /// 中断导航
    /// </summary>
    private void StopNavTask() {
        if (isNavGuide) {
            isNavGuide = false;
            nav.isStopped = true;
            nav.enabled = false;
            playerCtrl.SetBlend(Constants.BlendIdle);
        }
    }

    /// <summary>
    /// 打开引导界面
    /// </summary>
    private void OpenGuideWnd() {
        guideWnd.SetWndState();
    }

    /// <summary>
    /// 服务器回应
    /// </summary>
    /// <param name="msg"></param>
    public void RspGuide(GameMsg msg) {
        RspGuide data = msg.rspGuide;

        GameRoot.AddTips(Constants.Color("任务奖励 金币+" + curtTaskData.coin + "  经验+" + curtTaskData.exp, TxtColor.Blue));

        switch (curtTaskData.actID) {
            case 0:
                //与智者对话
                break;
            case 1:
                //TODO 进入副本
                break;
            case 2:
                //TODO 进入强化界面
                break;
            case 3:
                //TODO 进入体力购买
                break;
            case 4:
                //TODO 进入金币铸造
                break;
            case 5:
                //TODO 进入世界聊天
                break;
        }
        GameRoot.Instance.SetPlayerDataByGuide(data);
        mainCityWnd.RefreshUI();
    }
    #endregion

    #region 强化
    /// <summary>
    /// 打开强化界面
    /// </summary>
    public void OpenStrongWnd() {
        strongWnd.SetWndState();
    }

    /// <summary>
    /// 强化的服务器回应
    /// </summary>
    public void RspStrong(GameMsg msg) {
        int fightPre = PECommon.GetFightByProps(GameRoot.Instance.PlayerData); // 之前的战力
        GameRoot.Instance.SetPlayerDataByStrong(msg.rspStrong);
        int fightNow = PECommon.GetFightByProps(GameRoot.Instance.PlayerData); // 新的战力，强化后
        GameRoot.AddTips(Constants.Color("战力提升 " + (fightNow - fightPre), TxtColor.Blue));
        strongWnd.UpdateUI();
        mainCityWnd.RefreshUI();
    }
    #endregion

    #region 聊天
    public void OpenChatWnd() {
        chatWnd.SetWndState();
    }
    /// <summary>
    /// 服务器推送
    /// </summary>
    /// <param name="msg"></param>
    public void PshChat(GameMsg msg) {
        chatWnd.AddChatMsg(msg.pshChat.name, msg.pshChat.chat);
    }
    #endregion

    #region 购买
    public void OpenBuyWnd(int type) {
        buyWnd.SetBuyType(type);
        buyWnd.SetWndState();
    }

    public void RspBuy(GameMsg msg) {
        RspBuy data = msg.rspBuy;
        GameRoot.Instance.SetPlayerDataByBuy(data);
        GameRoot.AddTips("购买成功");
        mainCityWnd.RefreshUI();
        buyWnd.SetWndState(false);
    }

    public void PshPower(GameMsg msg) {
        PshPower data = msg.pshPower;
        GameRoot.Instance.SetPlayerDataByPower(data);
        if (mainCityWnd.gameObject.activeSelf) {
            mainCityWnd.RefreshUI();
        }
    }
    #endregion
}