/****************************************************
    文件：Constants.cs
	作者：CaptainYun
    日期：2019/5/12 22:30:0
	功能：常量配置
*****************************************************/

using UnityEngine;

/// <summary>
/// 文字颜色
/// </summary>
public enum TxtColor {
    Red,
    Green,
    Blue,
    Yellow
}

public class Constants {

    #region 颜色

    private const string ColorRed = "<color=#FF0000FF>";
    private const string ColorGreen = "<color=#00FF00FF>";
    private const string ColorBlue = "<color=#00B4FFFF>";
    private const string ColorYellow = "<color=#FFFF00FF>";
    private const string ColorEnd = "</color>";

    /// <summary>
    /// 设置颜色
    /// </summary>
    public static string Color(string str, TxtColor c) {
        string result = "";
        switch (c) {
            case TxtColor.Red:
                result = ColorRed + str + ColorEnd;
                break;
            case TxtColor.Green:
                result = ColorGreen + str + ColorEnd;
                break;
            case TxtColor.Blue:
                result = ColorBlue + str + ColorEnd;
                break;
            case TxtColor.Yellow:
                result = ColorYellow + str + ColorEnd;
                break;
        }
        return result;
    }
    #endregion

    // --------------- 场景名称/ID ----------------
    public const string SceneLogin = "SceneLogin";
    public const int MainCityMapID = 10000;
    //public const string SceneMainCity = "SceneMainCity";

    // --------------- BGM ----------------
    public const string BGLogin = "bgLogin"; // 登录场景的背景音乐
    public const string BGMainCity = "bgMainCity"; // 主城背景音乐
    public const string BGHuangYe = "bgHuangYe"; // 战斗副本

    // --------------- 音效 ----------------
    // 常规 UI 点击音效
    public const string UILoginBtn = "uiLoginBtn";  // 登录游戏音效
    public const string UIClickBtn = "uiClickBtn"; // 按钮点击音效
    public const string UIExtenBtn = "uiExtenBtn"; // 主界面菜单
    public const string UIOpenPage = "uiOpenPage"; // 打开窗口
    public const string FBItemEnter = "fbitem";

    // --------------- 移动速度 ----------------
    public const int PlayerMoveSpeed = 8; // 角色
    public const int MonsterMoveSpeed = 8; // 怪物

    // --------------- 角色 ----------------
    // 角色运动平滑加速度
    public const float AccelerSpeed = 5;
    // 混合参数
    public const int BlendIdle = 0;
    public const int BlendMove = 1;
    // Action 触发参数
    public const int ActionDefault = -1;

    // --------------- 其他 ----------------
    // 屏幕标准宽高
    public const int ScreenStandardWidth = 1334;
    public const int ScreenStandardHeight = 750;

    // 摇杆点标准距离
    public const int ScreenOPDis = 90;

    // 自动引导的 NPC ID
    public const int NPCWiseMan = 0;
    public const int NPCGeneral = 1;
    public const int NPCArtisan = 2;
    public const int NPCTrader = 3;

}