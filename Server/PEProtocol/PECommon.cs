/****************************************************
	文件：PECommon.cs
	作者：CaptainYun
	日期：2019/05/14 21:06   	
	功能：客户端服务端共用工具类
*****************************************************/

using PENet;
using PEProtocol;

/// <summary>
/// 日志类型
/// </summary>
public enum LogType {
    Log = 0,
    Warn = 1,
    Error = 2,
    Info = 3
}

/// <summary>
/// 客户端服务端共用工具类
/// </summary>
public class PECommon {

    /// <summary>
    /// 打印日志
    /// </summary>
    /// <param name="msg">日志内容</param>
    /// <param name="tp">日志级别</param>
    public static void Log(string msg = "", LogType tp = LogType.Log) {
        LogLevel lv = (LogLevel)tp;
        PETool.LogMsg(msg, lv);
    }

    /// <summary>
    /// 获取角色的战斗力
    /// </summary>
    /// <returns></returns>
    public static int GetFightByProps(PlayerData pd) {
        // 通过对玩家的属性进行计算，得出角色的战斗力
        return pd.lv * 100 + pd.ad + pd.ap + pd.addef + pd.apdef;
    }

    /// <summary>
    /// 获取体力上限
    /// </summary>
    /// <returns></returns>
    public static int GetPowerLimit(int lv) {
        return ((lv - 1) / 10) * 150 + 150;
    }

    /// <summary>
    /// 获取当前升级需要的经验值
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public static int GetExpUpValByLv(int lv) {
        return 100 * lv * lv;
    }

}
