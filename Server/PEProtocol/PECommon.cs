/****************************************************
	文件：PECommon.cs
	作者：CaptainYun
	日期：2019/05/14 21:06   	
	功能：客户端服务端共用工具类
*****************************************************/

using PENet;

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

}
