/****************************************************
    文件：NetSvc.cs
	作者：CaptainYun
    日期：2019/5/14 20:43:14
	功能：网络服务
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using PENet;
using PEProtocol;
using UnityEngine;

public class NetSvc : MonoBehaviour {
    // 单例
    public static NetSvc Instance = null;

    PESocket<ClientSession, GameMsg> client = null;
    // 消息队列
    private Queue<GameMsg> msgQue = new Queue<GameMsg>();

    /// <summary>
    /// 初始化服务
    /// </summary>
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

    /// <summary>
    /// 给服务器端发送消息
    /// </summary>
    public void SendMsg(GameMsg msg) {
        if (client.session != null) {
            client.session.SendMsg(msg);
        }
        else { // 说明网络服务的连接有问题
            GameRoot.AddTips("没有连接服务器");
            InitSvc(); // 重新初始化一次服务
        }
    }

    // 锁
    public static readonly string obj = "lock";

    /// <summary>
    /// 加入到消息队列
    /// </summary>
    /// <param name="msg"></param>
    public void AddNetPkg(GameMsg msg) {
        lock (obj) { // 因为是异步多线程的网络库，所以需要加锁
            msgQue.Enqueue(msg);
        }
    }

    private void Update() {
        if (msgQue.Count > 0) { // 当前队列里是否有数据
            lock (obj) {
                GameMsg msg = msgQue.Dequeue(); // 取出一条数据
                ProcessMsg(msg); // 处理数据
            }
        }
    }

    /// <summary>
    /// 处理数据，分发
    /// </summary>
    /// <param name="msg"></param>
    private void ProcessMsg(GameMsg msg) {
        // 接收到了错误码
        if (msg.err != (int)ErrorCode.None) {
            switch ((ErrorCode)msg.err) {
                case ErrorCode.UpdateDBError:
                    PECommon.Log("数据库更新异常", LogType.Error);
                    GameRoot.AddTips("网络不稳定");
                    break;
                case ErrorCode.ServerDataError:
                    PECommon.Log("客户端和服务器数据不一致", LogType.Error);
                    GameRoot.AddTips("网络不稳定");
                    break;
                case ErrorCode.WrongPwd:
                    GameRoot.AddTips("密码错误");
                    break;
                case ErrorCode.NameIsExist:
                    GameRoot.AddTips("用户名已存在");
                    break;
                case ErrorCode.AcctIsOnLine:
                    GameRoot.AddTips("用户已在线");
                    break;
                default:
                    break;
            }
            return;
        }
        // 分发处理接收消息
        switch ((CMD)msg.cmd) {
            // 登录回应
            case CMD.RspLogin:
                LoginSys.Instance.RspLogin(msg);
                break;
            // 改名回应
            case CMD.RspRename:
                LoginSys.Instance.RspRename(msg);
                break;
            // 引导任务
            case CMD.RspGuide:
                MainCitySys.Instance.RspGuide(msg);
                break;
            default:
                break;
        }
    }
}