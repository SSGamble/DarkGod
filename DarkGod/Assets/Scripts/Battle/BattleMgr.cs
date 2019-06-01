/****************************************************
	文件：BattleMgr.cs
	作者：CaptainYun
	日期：2019/06/01 22:37   	
	功能：战场管理器
*****************************************************/

using UnityEngine;

public class BattleMgr : MonoBehaviour {
    private ResSvc resSvc;
    private AudioSvc audioSvc;

    //private StateMgr stateMgr;
    //private SkillMgr skillMgr;
    //private MapMgr mapMgr;

    //private EntityPlayer entitySelfPlayer;

    //public void Init(int mapid) {
    //    resSvc = ResSvc.Instance;
    //    audioSvc = AudioSvc.Instance;

    //    //初始化各管理器
    //    stateMgr = gameObject.AddComponent<StateMgr>();
    //    stateMgr.Init();
    //    skillMgr = gameObject.AddComponent<SkillMgr>();
    //    skillMgr.Init();

    //    //加载战场地图
    //    MapCfg mapData = resSvc.GetMapCfg(mapid);
    //    resSvc.AsyncLoadScene(mapData.sceneName, () => {
    //        //初始化地图数据
    //        GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
    //        mapMgr = map.GetComponent<MapMgr>();
    //        mapMgr.Init();

    //        map.transform.localPosition = Vector3.zero;
    //        map.transform.localScale = Vector3.one;

    //        Camera.main.transform.position = mapData.mainCamPos;
    //        Camera.main.transform.localEulerAngles = mapData.mainCamRote;

    //        LoadPlayer(mapData);

    //        audioSvc.PlayBGMusic(Constants.BGHuangYe);
    //    });
    //}

    //private void LoadPlayer(MapCfg mapData) {
    //    GameObject player = resSvc.LoadPrefab(PathDefine.AssissnBattlePlayerPrefab);

    //    player.transform.position = mapData.playerBornPos;
    //    player.transform.localEulerAngles = mapData.playerBornRote;
    //    player.transform.localScale = Vector3.one;

    //    entitySelfPlayer = new EntityPlayer {
    //        stateMgr = stateMgr
    //    };

    //    PlayerController playerCtrl = player.GetComponent<PlayerController>();
    //    playerCtrl.Init();
    //    entitySelfPlayer.controller = playerCtrl;
    //}

    //public void SetSelfPlayerMoveDir(Vector2 dir) {
    //    //设置玩家移动
    //    //PECommon.Log(dir.ToString());
    //    if (dir == Vector2.zero) {
    //        entitySelfPlayer.Idle();
    //    }
    //    else {
    //        entitySelfPlayer.Move();
    //    }
    //}

    //public void ReqReleaseSkill(int index) {
    //    switch (index) {
    //        case 0:
    //            ReleaseNormalAtk();
    //            break;
    //        case 1:
    //            ReleaseSkill1();
    //            break;
    //        case 2:
    //            ReleaseSkill2();
    //            break;
    //        case 3:
    //            ReleaseSkill3();
    //            break;
    //    }
    //}


    //private void ReleaseNormalAtk() {
    //    PECommon.Log("Click Normal Atk");
    //}

    //private void ReleaseSkill1() {
    //    PECommon.Log("Click Skill1");
    //}
    //private void ReleaseSkill2() {
    //    PECommon.Log("Click Skill2");
    //}
    //private void ReleaseSkill3() {
    //    PECommon.Log("Click Skill3");
    //}
}