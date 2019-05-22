/****************************************************
	文件：BaseData.cs
	作者：CaptainYun
	日期：2019/05/21 17:40   	
	功能：配置数据类
*****************************************************/

using UnityEngine;

/// <summary>
/// 基类
/// </summary>
public class BaseData<T> {
    public int id;
}

/// <summary>
/// 地图
/// </summary>
public class MapCfg : BaseData<MapCfg> {
    public string mapName;
    public string sceneName;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;
}
