/****************************************************
    文件：Constants.cs
	作者：CaptainYun
    日期：2019/5/12 22:30:0
	功能：常量配置
*****************************************************/

using UnityEngine;

public class Constants {

    // --------------- 场景名称/ID ----------------
    public const string SceneLogin = "SceneLogin";
    public const int MainCityMapID = 10000;
    //public const string SceneMainCity = "SceneMainCity";

    // --------------- 音乐 ----------------
    public const string BGLogin = "bgLogin"; // 登录场景的背景音乐
    public const string BGMainCity = "bgMainCity"; // 主城背景音乐

    // --------------- 音效 ----------------
    // 常规 UI 点击音效
    public const string UILoginBtn = "uiLoginBtn";  // 登录游戏音效
    public const string UIClickBtn = "uiClickBtn"; // 按钮点击音效
    public const string UIExtenBtn = "uiExtenBtn"; // 主界面菜单
    public const string UIOpenPage = "uiOpenPage"; // 打开窗口

    // --------------- 移动速度 ----------------
    public const int PlayerMoveSpeed = 8; // 角色
    public const int MonsterMoveSpeed = 8; // 怪物

    // --------------- 其他 ----------------
    // 屏幕标准宽高
    public const int ScreenStanderWidth = 1334;
    public const int ScreenStanderHeight = 750;

    // 摇杆点标准距离
    public const int ScreenOPDis = 90;

    // 角色运动平滑加速度
    public const float AccelerSpeed = 5;
    // 混合参数
    public const int BlendIdle = 0;
    public const int BlendWalk = 1;

    // 自动引导的 NPC ID
    public const int NPCWiseMan = 0;
    public const int NPCGeneral = 1;
    public const int NPCArtisan = 2;
    public const int NPCTrader = 3;
}