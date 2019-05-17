/****************************************************
	文件：SreverStart.cs
	作者：CaptainYun
	日期：2019/05/14 19:59   	
	功能：服务器入口
*****************************************************/
class SreverStart {
    static void Main(string[] args) {
        ServerRoot.Instance.Init();

        // 防止进程退出
        while (true) {

        }
    }
}
