/****************************************************
    文件：MainCitySvc.cs
	作者：CaptainYun
    日期：2019/5/20 14:6:35
	功能：主城业务系统
*****************************************************/

using UnityEngine;

public class MainCitySys : SystemRoot {
    // 单例
    public static MainCitySys Instance = null;

    public MainCityWnd mainCityWnd;

    public override void InitSys() {
        base.InitSys();
        Instance = this;
        PECommon.Log("Init MainCitySvc..");
    }

    /// <summary>
    /// 进入主城
    /// </summary>
    public void EnterMainCity() {
        resSvc.AsyncLoadScene(Constants.SceneMainCity, () => {
            // 加载主角

            // 打开主城界面
            mainCityWnd.SetWndState();
            // 播放主城背景音乐
            audioSvc.PlayBGAudio(Constants.BGMainCity);

           // 设置角色显示相机
        });
    }
}