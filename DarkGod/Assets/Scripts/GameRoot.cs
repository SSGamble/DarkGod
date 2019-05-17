/****************************************************
    文件：GameRoot.cs
	作者：CaptainYun
    日期：2019/5/12 21:43:13
	功能：游戏的启动入口，初始化各个系统，保存核心数据
*****************************************************/

using UnityEngine;

public class GameRoot : MonoBehaviour {

    // 单例
    public static GameRoot Instance = null;
    // 加载进度界面
    public LoadingWnd loadingWnd;
    public DynamicWnd dynamicWnd;


    private void Start() {
        Instance = this;
        DontDestroyOnLoad(this); // 当前 GameRoot 不需要销毁
        PECommon.Log("Game Start...");
        ClearUIRoot();
        Init();
    }

    private void ClearUIRoot() {
        Transform canvas = transform.Find("Canvas");
        // 遍历 Canvas 下的所有子物体
        for (int i = 0; i < canvas.childCount; i++) {
            canvas.GetChild(i).gameObject.SetActive(false);
        }
        dynamicWnd.gameObject.SetActive(true);
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

        // 业务模块初始化
        LoginSys login = GetComponent<LoginSys>();
        login.InitSys();

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
}