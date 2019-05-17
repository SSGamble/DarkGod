/****************************************************
    文件：NetSvc.cs
	作者：CaptainYun
    日期：2019/5/14 20:43:14
	功能：网络服务
*****************************************************/

using PENet;
using PEProtocol;
using UnityEngine;

public class NetSvc : MonoBehaviour {
    // 单例
    public static NetSvc Instance = null;

    PESocket<ClientSession, GameMsg> client = null;

    public void InitSvc() {
        Instance = this;

        client = new PESocket<ClientSession, GameMsg>();
        // 设置日志接口
        client.SetLog(true, (string msg, int lv) => {
            switch (lv) {
                case 0:
                    msg = "Log:" + msg;
                    Debug.Log(msg);
                    break;
                case 1:
                    msg = "Warn:" + msg;
                    Debug.LogWarning(msg);
                    break;
                case 2:
                    msg = "Error:" + msg;
                    Debug.LogError(msg);
                    break;
                case 3:
                    msg = "Info:" + msg;
                    Debug.Log(msg);
                    break;
            }
        });
        // 创建 Socket 客户端
        client.StartAsClient(SrvCfg.srvIP, SrvCfg.srvPort);

        PECommon.Log("Init NetSvc...");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            client.session.SendMsg(new GameMsg {
                text = "hello unity"
            });
        }
    }
}