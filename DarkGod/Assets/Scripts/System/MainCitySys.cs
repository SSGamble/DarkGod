/****************************************************
    文件：MainCitySvc.cs
	作者：CaptainYun
    日期：2019/5/20 14:6:35
	功能：主城业务系统
*****************************************************/

using System;
using UnityEngine;

public class MainCitySys : SystemRoot {
    // 单例
    public static MainCitySys Instance = null;

    public MainCityWnd mainCityWnd;
    public InfoWnd infoWnd;

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
            // 设置角色显示相机
            if (charCamTrans != null) {
                charCamTrans.gameObject.SetActive(false);
            }
        });
    }

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
    }

    /// <summary>
    /// 控制角色移动
    /// </summary>
    /// <param name="dir"></param>
    public void SetMoveDir(Vector2 dir) {
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

    /// <summary>
    /// 打开角色信息窗口
    /// </summary>
    public void OpenInfoWnd() {
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
        if (charCamTrans!=null) {
            charCamTrans.gameObject.SetActive(false); // 关闭角色相机
            infoWnd.SetWndState(false);
        }
    }

    private float startRoate = 0; // 默认开始旋转角度
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
}