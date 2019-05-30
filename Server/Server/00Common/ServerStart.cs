/****************************************************
	文件：SreverStart.cs
	作者：CaptainYun
	日期：2019/05/14 19:59   	
	功能：服务器入口
*****************************************************/
using System.Threading;

class ServerStart {
    static void Main(string[] args) {

        ServerRoot.Instance.Init();

        // 防止进程退出
        while (true) {
            ServerRoot.Instance.Update();
            Thread.Sleep(20); // 死循环运行帧率极高，其实不需要这么高，休眠一下用于降低 CPU 消耗
        }
    }
}
