/****************************************************
	文件：PETools.cs
	作者：CaptainYun
	日期：2019/05/14 18:02   	
	功能：工具类
*****************************************************/

public class PETools {

    /// <summary>
    /// 获取一个随机数
    /// </summary>
    /// <param name="min">最小值（包括）</param>
    /// <param name="max">最大值（不包括）</param>
    /// <param name="ran">随机种子</param>
    /// <returns></returns>
    public static int RanInt(int min, int max, System.Random ran = null) {
        if (ran == null) {
            ran = new System.Random();
        }
        int val = ran.Next(min, max + 1);
        return val;
    }
}
