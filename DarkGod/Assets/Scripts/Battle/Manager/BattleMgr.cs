/****************************************************
	文件：BattleMgr.cs
	作者：CaptainYun
	日期：2019/06/01 22:37   	
	功能：战场管理器，管理具体的某一场战斗
*****************************************************/

using UnityEngine;

public class BattleMgr : MonoBehaviour {
    private ResSvc resSvc;
    private AudioSvc audioSvc;

    private StateMgr stateMgr;
    private SkillMgr skillMgr;
    private MapMgr mapMgr;

    private EntityPlayer entitySelfPlayer; // 玩家逻辑实体

    public void Init(int mapid) {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;

        // 初始化各管理器
        stateMgr = gameObject.AddComponent<StateMgr>();
        stateMgr.Init();
        skillMgr = gameObject.AddComponent<SkillMgr>();
        skillMgr.Init();

        // 加载战场地图
        MapCfg mapData = resSvc.GetMapCfg(mapid);
        resSvc.AsyncLoadScene(mapData.sceneName, () => {
            // 初始化地图数据
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            mapMgr = map.GetComponent<MapMgr>();
            mapMgr.Init();
            // 规范位置
            map.transform.localPosition = Vector3.zero;
            map.transform.localScale = Vector3.one;
            Camera.main.transform.position = mapData.mainCamPos;
            Camera.main.transform.localEulerAngles = mapData.mainCamRote;
            // 加载主角
            LoadPlayer(mapData);
            entitySelfPlayer.Idle(); // 默认 idle 状态
            audioSvc.PlayBGMusic(Constants.BGHuangYe);
        });
    }

    /// <summary>
    /// 加载主角
    /// </summary>
    private void LoadPlayer(MapCfg mapData) {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssissnBattlePlayerPrefab);
        // 初始化主角位置信息
        player.transform.position = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.localScale = Vector3.one;

        // 为逻辑实体注入管理器
        entitySelfPlayer = new EntityPlayer {
            stateMgr = stateMgr,
            skillMgr = skillMgr,
            battleMgr = this
        };

        // 为逻辑实体注入控制器
        PlayerController playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.Init();
        entitySelfPlayer.controller = playerCtrl;
    }

    /// <summary>
    /// 设置玩家移动
    /// </summary>
    public void SetSelfPlayerMoveDir(Vector2 dir) {
        if (entitySelfPlayer.canControl == false) {
            return;
        }
        if (dir == Vector2.zero) {
            entitySelfPlayer.Idle();
        }
        else {
            entitySelfPlayer.Move();
            entitySelfPlayer.SetDir(dir);
        }
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    public void ReqReleaseSkill(int index) {
        switch (index) {
            case 0:
                ReleaseNormalAtk();
                break;
            case 1:
                ReleaseSkill1();
                break;
            case 2:
                ReleaseSkill2();
                break;
            case 3:
                ReleaseSkill3();
                break;
        }
    }

    private void ReleaseNormalAtk() {
        PECommon.Log("Click Normal Atk");
    }

    private void ReleaseSkill1() {
        entitySelfPlayer.Attack(101);
    }

    private void ReleaseSkill2() {
        PECommon.Log("Click Skill2");
    }

    private void ReleaseSkill3() {
        PECommon.Log("Click Skill3");
    }

    public Vector2 GetDirInput() {
        return BattleSys.Instance.GetDirInput();
    }
}