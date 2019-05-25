/****************************************************
	文件：CfgSvc.cs
	作者：CaptainYun
	日期：2019/05/24 19:43   	
	功能：配置数据服务
*****************************************************/
using System.Xml;
using System.Collections.Generic;
using System;

public class GuideCfg : BaseData<GuideCfg> {
    public int coin;
    public int exp;
}

public class BaseData<T> {
    public int id;
}

public class CfgSvc {
    private static CfgSvc instance = null;
    public static CfgSvc Instance {
        get {
            if (instance == null) {
                instance = new CfgSvc();
            }
            return instance;
        }
    }

    public void Init() {
        InitGuideCfg();
        PECommon.Log("CfgSvc Init Done.");
    }

    #region 自动引导配置
    private Dictionary<int, GuideCfg> guideDic = new Dictionary<int, GuideCfg>();
    private void InitGuideCfg() {
        // 解析 xml 文件
        XmlDocument doc = new XmlDocument();
        
        doc.Load(@"C:\guide.xml");
        //doc.Load(@"G:\UnityDocuments\DarkGod\DarkGod\Assets\Resources\ResCfgs\guide.xml");

        XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodLst.Count; i++) {
            XmlElement ele = nodLst[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null) {
                continue;
            }
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            GuideCfg mc = new GuideCfg {
                id = ID
            };

            foreach (XmlElement e in nodLst[i].ChildNodes) {
                switch (e.Name) {
                    case "coin":
                        mc.coin = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        mc.exp = int.Parse(e.InnerText);
                        break;
                }
            }
            guideDic.Add(ID, mc);
        }
    }

    /// <summary>
    /// 获得引导数据
    /// </summary>
    public GuideCfg GetGuideData(int id) {
        GuideCfg agc = null;
        if (guideDic.TryGetValue(id, out agc)) {
            return agc;
        }
        return null;
    }

    #endregion
}
