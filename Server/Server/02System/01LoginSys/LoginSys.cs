/****************************************************
	文件：LoginSys.cs
	作者：CaptainYun
	日期：2019/05/14 20:07   	
	功能：登录业务系统
*****************************************************/

public class LoginSys {
    private static LoginSys instance = null;
    public static LoginSys Instance {
        get {
            if (instance == null) {
                instance = new LoginSys();
            }
            return instance;
        }
    }

    public void Init() {
        PECommon.Log("LoginSys Init Done.");
    }
}
