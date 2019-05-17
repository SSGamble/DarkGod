/****************************************************
    文件：ClientSession.cs
	作者：CaptainYun
    日期：2019/5/14 20:48:27
	功能：Nothing
*****************************************************/

using PENet;
using PEProtocol;
using UnityEngine;

public class ClientSession : PESession<GameMsg> {

    protected override void OnConnected() {
        PETool.LogMsg("Server Connect");
    }

    protected override void OnReciveMsg(GameMsg msg) {
        PETool.LogMsg("Server Rsp: " + msg.text);
    }

    protected override void OnDisConnected() {
        PETool.LogMsg("Server DisConnect");
    }
}