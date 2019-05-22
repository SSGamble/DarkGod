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
        InitMapCfg(PathDefine.MapCfg);
        PECommon.Log("Init ResSvc..");
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

    // 预制体缓存
    public Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
    /// <summary>
    /// 加载预制体
    /// </summary>
    public GameObject LoadPrefab(string path, bool cache = false) {
        GameObject prefab = null;
        if (!goDic.TryGetValue(path,out prefab)) {
            prefab = Resources.Load<GameObject>(path);
            if (cache) {
                goDic.Add(path, prefab);
            }
        }
        GameObject go = null;
        if (prefab!=null) {
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
                int id = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
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
                    id = id
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
                    }
                }
                mapCfgDataDic.Add(id, mc); // 将地图数据添加到字典里
            }
        }
    }

    /// <summary>
    /// 获取地图数据
    /// </summary>
    public MapCfg GetMapCfgData(int id) {
        MapCfg data;
        if (mapCfgDataDic.TryGetValue(id,out data)) {
            return data;
        }
        return null;
    }
    #endregion

    #endregion

}