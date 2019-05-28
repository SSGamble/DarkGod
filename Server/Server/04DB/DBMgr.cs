/****************************************************
	文件：DBMgr.cs
	作者：CaptainYun
	日期：2019/05/19 14:54   	
	功能：数据库管理类
*****************************************************/
using MySql.Data.MySqlClient;
using PEProtocol;

public class DBMgr {
    // 单例
    private static DBMgr instance = null;
    public static DBMgr Instance {
        get {
            if (instance == null) {
                instance = new DBMgr();
            }
            return instance;
        }
    }

    private MySqlConnection conn; // 数据库连接

    public void Init() {
        conn = new MySqlConnection("server=localhost;User Id=root;password=root;Database=darkgod;Charset=utf8");
        conn.Open();
        PECommon.Log("DBMgr Init Done.");
    }

    /// <summary>
    /// 查询账号信息
    /// </summary>
    /// <param name="acct"></param>
    /// <param name="pwd"></param>
    /// <returns></returns>
    public PlayerData QueryPlayerData(string acct, string pwd) {
        bool isNew = true; // 是否是新的账号
        PlayerData playerData = null;
        MySqlDataReader reader = null;
        try {
            MySqlCommand cmd = new MySqlCommand("select * from account where acct = @acct", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            reader = cmd.ExecuteReader();

            if (reader.Read()) {
                isNew = false;
                string _pwd = reader.GetString("pwd");
                if (_pwd.Equals(pwd)) {
                    // 密码正确，返回玩家数据
                    playerData = new PlayerData {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("name"),
                        lv = reader.GetInt32("level"),
                        exp = reader.GetInt32("exp"),
                        power = reader.GetInt32("power"),
                        coin = reader.GetInt32("coin"),
                        diamond = reader.GetInt32("diamond"),
                        crystal = reader.GetInt32("crystal"),
                        hp = reader.GetInt32("hp"),
                        ad = reader.GetInt32("ad"),
                        ap = reader.GetInt32("ap"),
                        addef = reader.GetInt32("addef"),
                        apdef = reader.GetInt32("apdef"),
                        dodge = reader.GetInt32("dodge"),
                        pierce = reader.GetInt32("pierce"),
                        critical = reader.GetInt32("critical"),
                        guideid = reader.GetInt32("guideid")
                    };
                    #region 解析 Strong
                    // 数据库里存的字符串，拿出来再进行分割
                    // 数据示意：1#2#2#4#3#7#                        
                    string[] strongStrArr = reader.GetString("strong").Split('#');

                    int[] _strongArr = new int[6];
                    for (int i = 0; i < strongStrArr.Length; i++) {
                        if (strongStrArr[i] == "") {
                            continue;
                        }
                        if (int.TryParse(strongStrArr[i], out int starLv)) {
                            _strongArr[i] = starLv;
                        }
                        else {
                            PECommon.Log("Parse Strong Data Error", LogType.Error);
                        }
                    }
                    playerData.strongArr = _strongArr;
                    #endregion
                }
            }
        }
        catch (System.Exception e) {
            PECommon.Log("查询账号信息出错," + e, LogType.Error);
        }
        finally {
            if (reader != null) {
                reader.Close();
            }
            if (isNew) {
                // 不存在账号数据，创建新的默认账号数据，并返回
                playerData = new PlayerData {
                    id = -1,
                    name = "",
                    lv = 1,
                    exp = 0,
                    power = 150,
                    coin = 5000,
                    diamond = 500,
                    crystal = 500,
                    hp = 2000,
                    ad = 275,
                    ap = 265,
                    addef = 67,
                    apdef = 43,
                    dodge = 7,
                    pierce = 5,
                    critical = 2,
                    guideid=1001,
                    strongArr = new int[6],
                };
                playerData.id = InsAcct(acct, pwd, playerData); // 赋予插入数据的 id
            }
        }
        return playerData;
    }

    /// <summary>
    /// 插入账号，返回 id
    /// </summary>
    private int InsAcct(string acct, string pwd, PlayerData pd) {
        int id = -1;
        string sql = "insert into account set acct=@acct,pwd =@pwd,name=@name,level=@level,exp=@exp,power=@power,coin=@coin," +
            "diamond=@diamond,crystal=@crystal,hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce," +
            "critical=@critical,guideid=@guideid,strong=@strong";
        try {
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("acct", acct);
            cmd.Parameters.AddWithValue("pwd", pwd);
            cmd.Parameters.AddWithValue("name", pd.name);
            cmd.Parameters.AddWithValue("level", pd.lv);
            cmd.Parameters.AddWithValue("exp", pd.exp);
            cmd.Parameters.AddWithValue("power", pd.power);
            cmd.Parameters.AddWithValue("coin", pd.coin);
            cmd.Parameters.AddWithValue("diamond", pd.diamond);
            cmd.Parameters.AddWithValue("crystal", pd.crystal);
            cmd.Parameters.AddWithValue("hp", pd.hp);
            cmd.Parameters.AddWithValue("ad", pd.ad);
            cmd.Parameters.AddWithValue("ap", pd.ap);
            cmd.Parameters.AddWithValue("addef", pd.addef);
            cmd.Parameters.AddWithValue("apdef", pd.apdef);
            cmd.Parameters.AddWithValue("dodge", pd.dodge);
            cmd.Parameters.AddWithValue("pierce", pd.pierce);
            cmd.Parameters.AddWithValue("critical", pd.critical);
            cmd.Parameters.AddWithValue("guideid", pd.guideid);
            string strongInfo = "";
            for (int i = 0; i < pd.strongArr.Length; i++) {
                strongInfo += pd.strongArr[i];
                strongInfo += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongInfo);
            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId; // 新插入数据的主键
        }
        catch (System.Exception e) {
            PECommon.Log("插入账号出错," + e, LogType.Error);
        }
        return id;
    }

    /// <summary>
    /// 名字是否已经存在
    /// </summary>
    /// <returns></returns>
    public bool QueryNameData(string name) {
        bool exist = false;
        MySqlDataReader reader = null;
        try {
            //名字是否已经存在 
            MySqlCommand cmd = new MySqlCommand("select * from account where name= @name", conn);
            cmd.Parameters.AddWithValue("name", name);
            reader = cmd.ExecuteReader();
            if (reader.Read()) {
                exist = true;
            }
        }
        catch (System.Exception e) {
            PECommon.Log("名字是否已经存在出错," + e, LogType.Error);
        }
        finally {
            if (reader != null) {
                reader.Close();
            }
        }
        return exist;
    }

    /// <summary>
    /// 更新玩家数据
    /// </summary>
    /// <param name="id"></param>
    /// <param name="playerData"></param>
    /// <returns></returns>
    public bool UpdataPlayerData(int id, PlayerData playerData) {
        try {
            string sql = "update account set name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal," +
                "hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical," +
                "guideid=@guideid,strong=@strong" +
                " where id =@id";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("level", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("power", playerData.power);
            cmd.Parameters.AddWithValue("coin", playerData.coin);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.crystal);
            cmd.Parameters.AddWithValue("hp", playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.addef);
            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);
            cmd.Parameters.AddWithValue("guideid", playerData.guideid);
            string strongInfo = "";
            for (int i = 0; i < playerData.strongArr.Length; i++) {
                strongInfo += playerData.strongArr[i];
                strongInfo += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongInfo);
            cmd.ExecuteNonQuery();
        }
        catch (System.Exception e) {
            PECommon.Log("更新玩家数据出错," + e, LogType.Error);
            return false;
        }
        return true;
    }

}
