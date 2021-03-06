/****************************************************
    文件：ResSvc.cs
	作者：CaptainYun
    日期：2019/5/12 21:46:36
	功能：资源加载服务
*****************************************************/

using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResSvc : MonoBehaviour {

    // 单例
    public static ResSvc Instance = null;

    /// <summary>
    /// 初始化资源加载服务
    /// </summary>
    public void InitSvc() {
        Instance = this;
        InitRanName(PathDefine.RanNameCfg);
        InitMonsterCfg(PathDefine.MonsterCfg);
        InitMapCfg(PathDefine.MapCfg);
        InitGuideCfg(PathDefine.GuideCfg);
        InitStrongCfg(PathDefine.StrongCfg);
        InitTaskRewardCfg(PathDefine.TaskRewardCfg);
        InitSkillCfg(PathDefine.SkillCfg);
        InitSkillMoveCfg(PathDefine.SkillMoveCfg);
        InitSkillActionCfg(PathDefine.SkillActionCfg);
        PECommon.Log("Init ResSvc..");
    }

    /// <summary>
    /// 重置技能配置文件
    /// </summary>
    public void ResetSkillCfgs() {
        skillDic.Clear();
        InitSkillCfg(PathDefine.SkillCfg);
        skillMoveDic.Clear();
        InitSkillMoveCfg(PathDefine.SkillMoveCfg);
        skillActionDic.Clear();
        InitSkillActionCfg(PathDefine.SkillActionCfg);
        PECommon.Log("Reset SkillCfgs...");
    }

    internal MapCfg GetMapCfgData(object mainCityMapID) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 更新的回调
    /// </summary>
    private Action prgCB = null;

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="loaded">场景加载完成后的回调</param>
    public void AsyncLoadScene(string sceneName, Action loaded) {
        // 显示加载进度界面，初始化加载进度界面
        GameRoot.Instance.loadingWnd.SetWndState();
        // 异步的加载登录场景
        AsyncOperation sceneAsyn = SceneManager.LoadSceneAsync(sceneName); // 异步加载场景
        // 动态显示加载进度条
        prgCB = () => {
            float prg = sceneAsyn.progress; // 当前起步加载场景的进度
            GameRoot.Instance.loadingWnd.SetProgress(prg); // 给加载进度界面设置进度数值
            // 加载完成                                               
            if (prg == 1) {
                // 如果加载完成后的回调不为空，就执行指定的方法
                if (loaded != null) {
                    loaded();
                }
                prgCB = null;
                sceneAsyn = null;
                GameRoot.Instance.loadingWnd.SetWndState(false);
            }
        };
    }

    private void Update() {
        //只要没加载完就一直去获取最新的进度
        if (prgCB != null) {
            prgCB();
        }
    }

    //缓存已加载的音乐
    private Dictionary<string, AudioClip> acDic = new Dictionary<string, AudioClip>();
    /// <summary>
    /// 加载声音资源
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="cache">是否缓存</param>
    /// <returns></returns>
    public AudioClip LoadAudio(string path, bool cache = false) {
        AudioClip ac = null;
        // 是否已经缓存，如果找到就直接 out 出去了
        if (!acDic.TryGetValue(path, out ac)) { // 没有缓存
            ac = Resources.Load<AudioClip>(path);
            // 如果需要缓存，就加进字典
            if (cache) {
                acDic.Add(path, ac);
            }
        }
        return ac;
    }

    // 精灵缓存
    private Dictionary<string, Sprite> spDic = new Dictionary<string, Sprite>();
    /// <summary>
    /// 加载图片
    /// </summary>
    public Sprite LoadSprite(string path, bool cache = false) {
        Sprite sp = null;
        if (!spDic.TryGetValue(path, out sp)) {
            sp = Resources.Load<Sprite>(path);
            if (cache) {
                spDic.Add(path, sp);
            }
        }
        return sp;
    }

    // 预制体缓存
    public Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
    /// <summary>
    /// 加载预制体
    /// </summary>
    public GameObject LoadPrefab(string path, bool cache = false) {
        GameObject prefab = null;
        if (!goDic.TryGetValue(path, out prefab)) {
            prefab = Resources.Load<GameObject>(path);
            if (cache) {
                goDic.Add(path, prefab);
            }
        }
        GameObject go = null;
        if (prefab != null) {
            go = Instantiate(prefab);
        }
        return go;
    }

    #region 加载配置文件

    #region 加载随机名字的配置文件

    private List<string> surnameLst = new List<string>();
    private List<string> manLst = new List<string>();
    private List<string> womanLst = new List<string>();

    private void InitRanName(string path) {
        // 读取 xml 文件
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml) {
            PECommon.Log(path + "不存在", LogType.Error);
        }
        else {
            // 读取 xml 文件
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            // 解析 xml 文件
            // 获取根节点下的所有子节点的 List
            XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
            // 遍历子节点
            for (int i = 0; i < nodeList.Count; i++) {
                // 将某一个节点转化为一个 XmlElement
                XmlElement ele = nodeList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                // 从 XmlElement 里获取名称为 “ID” 的数据
                //int id = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                // 遍历每一个 item 里的数据项
                foreach (XmlElement e in nodeList[i].ChildNodes) {
                    switch (e.Name) {
                        case "surname":
                            surnameLst.Add(e.InnerText);
                            break;
                        case "man":
                            manLst.Add(e.InnerText);
                            break;
                        case "woman":
                            womanLst.Add(e.InnerText);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获取一个随机的名字
    /// </summary>
    /// <param name="man">默认男人名字，false 为女人</param>
    /// <returns></returns>
    public string GetRanName(bool man = true) {
        string ranName = surnameLst[PETools.RanInt(0, surnameLst.Count - 1)];
        if (man) {
            ranName += manLst[PETools.RanInt(0, manLst.Count - 1)];
        }
        else {
            ranName += womanLst[PETools.RanInt(0, womanLst.Count - 1)];
        }
        return ranName;
    }
    #endregion

    #region 地图

    private Dictionary<int, MapCfg> mapCfgDataDic = new Dictionary<int, MapCfg>();

    private void InitMapCfg(string path) {
        // 读取 xml 文件
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml) {
            PECommon.Log(path + "不存在", LogType.Error);
        }
        else {
            // 读取 xml 文件
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            // 解析 xml 文件
            // 获取根节点下的所有子节点的 List
            XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
            // 遍历子节点
            for (int i = 0; i < nodeList.Count; i++) {
                // 将某一个节点转化为一个 XmlElement
                XmlElement ele = nodeList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                // 从 XmlElement 里获取名称为 “ID” 的数据
                int id = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                MapCfg mc = new MapCfg {
                    id = id,
                    monsterLst = new List<MonsterData>()
                };
                // 遍历每一个 item 里的数据项
                foreach (XmlElement e in nodeList[i].ChildNodes) {
                    switch (e.Name) {
                        case "mapName":
                            mc.mapName = e.InnerText;
                            break;
                        case "sceneName":
                            mc.sceneName = e.InnerText;
                            break;
                        case "power":
                            mc.power = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            mc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            mc.exp = int.Parse(e.InnerText);
                            break;
                        case "crystal":
                            mc.crystal = int.Parse(e.InnerText);
                            break;
                        case "mainCamPos": {
                                string[] valArr = e.InnerText.Split(',');
                                mc.mainCamPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                            break;
                        case "mainCamRote": {
                                string[] valArr = e.InnerText.Split(',');
                                mc.mainCamRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                            break;
                        case "playerBornPos": {
                                string[] valArr = e.InnerText.Split(',');
                                mc.playerBornPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                            break;
                        case "playerBornRote": {
                                string[] valArr = e.InnerText.Split(',');
                                mc.playerBornRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                            break;
                        case "monsterLst": {
                                string[] valArr = e.InnerText.Split('#'); // 批次
                                for (int waveIndex = 0; waveIndex < valArr.Length; waveIndex++) {
                                    if (waveIndex == 0) {
                                        continue;
                                    }
                                    string[] tempArr = valArr[waveIndex].Split('|'); // 哪个怪物
                                    for (int j = 0; j < tempArr.Length; j++) {
                                        if (j == 0) {
                                            continue;
                                        }
                                        string[] arr = tempArr[j].Split(',');
                                        MonsterData md = new MonsterData {
                                            id = int.Parse(arr[0]),
                                            mWave = waveIndex,
                                            mIndex = j,
                                            mCfg = GetMonsterCfg(int.Parse(arr[0])),
                                            mBornPos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3])),
                                            mBornRote = new Vector3(0, float.Parse(arr[4]), 0),
                                            mLevel = int.Parse(arr[5])
                                        };
                                        mc.monsterLst.Add(md);
                                    }
                                }
                            }
                            break;
                    }
                }
                mapCfgDataDic.Add(id, mc); // 将地图数据添加到字典里
            }
        }
    }

    /// <summary>
    /// 获取地图数据
    /// </summary>
    public MapCfg GetMapCfg(int id) {
        MapCfg data;
        if (mapCfgDataDic.TryGetValue(id, out data)) {
            return data;
        }
        return null;
    }
    #endregion

    #region 自动引导
    private Dictionary<int, AutoGuideCfg> guideTaskDic = new Dictionary<int, AutoGuideCfg>();
    private void InitGuideCfg(string path) {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml) {
            PECommon.Log("xml file:" + path + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                AutoGuideCfg mc = new AutoGuideCfg {
                    id = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "npcID":
                            mc.npcID = int.Parse(e.InnerText);
                            break;
                        case "dilogArr":
                            mc.dilogArr = e.InnerText;
                            break;
                        case "actID":
                            mc.actID = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            mc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            mc.exp = int.Parse(e.InnerText);
                            break;
                    }
                }
                guideTaskDic.Add(ID, mc);
            }
        }
    }
    public AutoGuideCfg GetAutoGuideCfg(int id) {
        AutoGuideCfg agc = null;
        if (guideTaskDic.TryGetValue(id, out agc)) {
            return agc;
        }
        return null;
    }
    #endregion

    #region 强化升级
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
    private void InitStrongCfg(string path) {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml) {
            PECommon.Log("xml file:" + path + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                StrongCfg sd = new StrongCfg {
                    id = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    int val = int.Parse(e.InnerText);
                    switch (e.Name) {
                        case "pos":
                            sd.pos = val;
                            break;
                        case "starlv":
                            sd.startlv = val;
                            break;
                        case "addhp":
                            sd.addhp = val;
                            break;
                        case "addhurt":
                            sd.addhurt = val;
                            break;
                        case "adddef":
                            sd.adddef = val;
                            break;
                        case "minlv":
                            sd.minlv = val;
                            break;
                        case "coin":
                            sd.coin = val;
                            break;
                        case "crystal":
                            sd.crystal = val;
                            break;
                    }
                }

                Dictionary<int, StrongCfg> dic = null;
                if (strongDic.TryGetValue(sd.pos, out dic)) {
                    dic.Add(sd.startlv, sd);
                }
                else {
                    dic = new Dictionary<int, StrongCfg>();
                    dic.Add(sd.startlv, sd);

                    strongDic.Add(sd.pos, dic);
                }
            }
        }
    }

    public StrongCfg GetStrongCfg(int pos, int starlv) {
        StrongCfg sd = null;
        Dictionary<int, StrongCfg> dic = null;
        if (strongDic.TryGetValue(pos, out dic)) {
            if (dic.ContainsKey(starlv)) {
                sd = dic[starlv];
            }
        }
        return sd;
    }

    /// <summary>
    /// 获取当前星级和之前星级的属性和
    /// </summary>
    public int GetPropAddValPreLv(int pos, int starlv, int type) {
        Dictionary<int, StrongCfg> posDic = null;
        int val = 0;
        if (strongDic.TryGetValue(pos, out posDic)) {
            for (int i = 0; i < starlv; i++) {
                StrongCfg sd;
                if (posDic.TryGetValue(i, out sd)) {
                    switch (type) {
                        case 1: //hp
                            val += sd.addhp;
                            break;
                        case 2: //hurt
                            val += sd.addhurt;
                            break;
                        case 3: //def
                            val += sd.adddef;
                            break;
                    }
                }
            }
        }
        return val;
    }
    #endregion

    #region 任务奖励配置
    private Dictionary<int, TaskRewardCfg> taskRewareDic = new Dictionary<int, TaskRewardCfg>();
    private void InitTaskRewardCfg(string path) {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml) {
            PECommon.Log("xml file:" + path + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                TaskRewardCfg trc = new TaskRewardCfg {
                    id = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "taskName":
                            trc.taskName = e.InnerText;
                            break;
                        case "count":
                            trc.count = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            trc.exp = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            trc.coin = int.Parse(e.InnerText);
                            break;
                    }
                }
                taskRewareDic.Add(ID, trc);
            }
        }
    }
    public TaskRewardCfg GetTaskRewardCfg(int id) {
        TaskRewardCfg trc = null;
        if (taskRewareDic.TryGetValue(id, out trc)) {
            return trc;
        }
        return null;
    }
    #endregion

    #region 技能配置
    private Dictionary<int, SkillCfg> skillDic = new Dictionary<int, SkillCfg>();
    private void InitSkillCfg(string path) {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml) {
            PECommon.Log("xml file:" + path + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                SkillCfg sc = new SkillCfg {
                    id = ID,
                    skillMoveLst = new List<int>(),
                    skillActionLst = new List<int>(),
                    skillDamageLst = new List<int>()
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "skillName":
                            sc.skillName = e.InnerText;
                            break;
                        case "cdTime":
                            sc.cdTime = int.Parse(e.InnerText);
                            break;
                        case "skillTime":
                            sc.skillTime = int.Parse(e.InnerText);
                            break;
                        case "aniAction":
                            sc.aniAction = int.Parse(e.InnerText);
                            break;
                        case "fx":
                            sc.fx = e.InnerText;
                            break;
                        case "isCombo":
                            sc.isCombo = e.InnerText.Equals("1");
                            break;
                        case "isCollide":
                            sc.isCollide = e.InnerText.Equals("1");
                            break;
                        case "isBreak":
                            sc.isBreak = e.InnerText.Equals("1");
                            break;
                        case "dmgType":
                            if (e.InnerText.Equals("1")) {
                                sc.dmgType = DamageType.AD;
                            }
                            else if (e.InnerText.Equals("2")) {
                                sc.dmgType = DamageType.AP;
                            }
                            else {
                                PECommon.Log("dmgType ERROR");
                            }
                            break;
                        case "skillMoveLst": // 技能位移分阶段，1011|1012
                            string[] skMoveArr = e.InnerText.Split('|');
                            for (int j = 0; j < skMoveArr.Length; j++) {
                                if (skMoveArr[j] != "") {
                                    sc.skillMoveLst.Add(int.Parse(skMoveArr[j]));
                                }
                            }
                            break;
                        case "skillActionLst":
                            string[] skActionArr = e.InnerText.Split('|');
                            for (int j = 0; j < skActionArr.Length; j++) {
                                if (skActionArr[j] != "") {
                                    sc.skillActionLst.Add(int.Parse(skActionArr[j]));
                                }
                            }
                            break;
                        case "skillDamageLst":
                            string[] skDamageArr = e.InnerText.Split('|');
                            for (int j = 0; j < skDamageArr.Length; j++) {
                                if (skDamageArr[j] != "") {
                                    sc.skillDamageLst.Add(int.Parse(skDamageArr[j]));
                                }
                            }
                            break;
                    }
                }
                skillDic.Add(ID, sc);
            }
        }
    }
    public SkillCfg GetSkillCfg(int id) {
        SkillCfg sc = null;
        if (skillDic.TryGetValue(id, out sc)) {
            return sc;
        }
        return null;
    }


    #region 技能移动
    private Dictionary<int, SkillMoveCfg> skillMoveDic = new Dictionary<int, SkillMoveCfg>();
    private void InitSkillMoveCfg(string path) {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml) {
            PECommon.Log("xml file:" + path + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                SkillMoveCfg smc = new SkillMoveCfg {
                    id = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "delayTime":
                            smc.delayTime = int.Parse(e.InnerText);
                            break;
                        case "moveTime":
                            smc.moveTime = int.Parse(e.InnerText);
                            break;
                        case "moveDis":
                            smc.moveDis = float.Parse(e.InnerText);
                            break;
                    }
                }
                skillMoveDic.Add(ID, smc);
            }
        }
    }
    public SkillMoveCfg GetSkillMoveCfg(int id) {
        SkillMoveCfg smc = null;
        if (skillMoveDic.TryGetValue(id, out smc)) {
            return smc;
        }
        return null;
    }
    #endregion

    #region 技能伤害点
    private Dictionary<int, SkillActionCfg> skillActionDic = new Dictionary<int, SkillActionCfg>();
    private void InitSkillActionCfg(string path) {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml) {
            PECommon.Log("xml file:" + path + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                SkillActionCfg sac = new SkillActionCfg {
                    id = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "delayTime":
                            sac.delayTime = int.Parse(e.InnerText);
                            break;
                        case "radius":
                            sac.radius = float.Parse(e.InnerText);
                            break;
                        case "angle":
                            sac.angle = int.Parse(e.InnerText);
                            break;
                    }
                }
                skillActionDic.Add(ID, sac);
            }
        }
    }
    public SkillActionCfg GetSkillActionCfg(int id) {
        SkillActionCfg sac = null;
        if (skillActionDic.TryGetValue(id, out sac)) {
            return sac;
        }
        return null;
    }
    #endregion
    #endregion

    #region 怪物
    private Dictionary<int, MonsterCfg> monsterCfgDataDic = new Dictionary<int, MonsterCfg>();
    private void InitMonsterCfg(string path) {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml) {
            PECommon.Log("xml file:" + path + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                MonsterCfg mc = new MonsterCfg {
                    id = ID,
                    bps = new BattleProps()
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "mName":
                            mc.mName = e.InnerText;
                            break;
                        case "mType":
                            if (e.InnerText.Equals("1")) {
                                mc.mType = MonsterType.Normal;
                            }
                            else if (e.InnerText.Equals("2")) {
                                mc.mType = MonsterType.Boss;
                            }
                            break;
                        case "isStop":
                            mc.isStop = int.Parse(e.InnerText) == 1;
                            break;
                        case "resPath":
                            mc.resPath = e.InnerText;
                            break;
                        case "skillID":
                            mc.skillID = int.Parse(e.InnerText);
                            break;
                        case "atkDis":
                            mc.atkDis = float.Parse(e.InnerText);
                            break;
                        case "hp":
                            mc.bps.hp = int.Parse(e.InnerText);
                            break;
                        case "ad":
                            mc.bps.ad = int.Parse(e.InnerText);
                            break;
                        case "ap":
                            mc.bps.ap = int.Parse(e.InnerText);
                            break;
                        case "addef":
                            mc.bps.addef = int.Parse(e.InnerText);
                            break;
                        case "apdef":
                            mc.bps.apdef = int.Parse(e.InnerText);
                            break;
                        case "dodge":
                            mc.bps.dodge = int.Parse(e.InnerText);
                            break;
                        case "pierce":
                            mc.bps.pierce = int.Parse(e.InnerText);
                            break;
                        case "critical":
                            mc.bps.critical = int.Parse(e.InnerText);
                            break;
                    }
                }
                monsterCfgDataDic.Add(ID, mc);
            }
        }
    }
    public MonsterCfg GetMonsterCfg(int id) {
        MonsterCfg data;
        if (monsterCfgDataDic.TryGetValue(id, out data)) {
            return data;
        }
        return null;
    }
    #endregion

    #endregion

}