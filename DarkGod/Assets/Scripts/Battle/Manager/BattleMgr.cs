/****************************************************
	文件：BattleMgr.cs
	作者：CaptainYun
	日期：2019/06/01 22:37   	
	功能：战场管理器，管理具体的某一场战斗
*****************************************************/

using System.Collections.Generic;
using PEProtocol;
using UnityEngine;

public class BattleMgr : MonoBehaviour {
    private ResSvc resSvc;
    private AudioSvc audioSvc;

    private StateMgr stateMgr;
    private SkillMgr skillMgr;
    private MapMgr mapMgr;

    private EntityPlayer entitySelfPlayer; // 玩家逻辑实体
    private MapCfg mapCfg;

    // 场景里所有的怪物实体
    private Dictionary<string, EntityMonster> monsterDic = new Dictionary<string, EntityMonster>();

    public void Init(int mapid) {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;

        // 初始化各管理器
        stateMgr = gameObject.AddComponent<StateMgr>();
        stateMgr.Init();
        skillMgr = gameObject.AddComponent<SkillMgr>();
        skillMgr.Init();

        // 加载战场地图
        mapCfg = resSvc.GetMapCfg(mapid);
        resSvc.AsyncLoadScene(mapCfg.sceneName, () => {
            // 初始化地图数据
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            mapMgr = map.GetComponent<MapMgr>();
            mapMgr.Init(this);
            // 规范位置
            map.transform.localPosition = Vector3.zero;
            map.transform.localScale = Vector3.one;
            Camera.main.transform.position = mapCfg.mainCamPos;
            Camera.main.transform.localEulerAngles = mapCfg.mainCamRote;
            // 加载主角
            LoadPlayer(mapCfg);
            entitySelfPlayer.Idle(); // 默认 idle 状态
            // 激活第一批怪物
            ActiveCurrentBatchMonsters();
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
        // 获取角色的战斗属性
        PlayerData pd = GameRoot.Instance.PlayerData;
        BattleProps props = new BattleProps {
            hp = pd.hp,
            ad = pd.ad,
            ap = pd.ap,
            addef = pd.addef,
            apdef = pd.apdef,
            dodge = pd.dodge,
            pierce = pd.pierce,
            critical = pd.critical
        };

        // 为逻辑实体注入管理器
        entitySelfPlayer = new EntityPlayer {
            stateMgr = stateMgr,
            skillMgr = skillMgr,
            battleMgr = this
        };
        entitySelfPlayer.Name = "主角名";
        // 为逻辑实体设置战斗属性
        entitySelfPlayer.SetBattleProps(props);
        // 为逻辑实体注入控制器
        PlayerController playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.Init();
        entitySelfPlayer.SetController(playerCtrl);
    }

    /// <summary>
    /// 加载怪物
    /// </summary>
    public void LoadMonsterByWaveID(int wave) {
        for (int i = 0; i < mapCfg.monsterLst.Count; i++) {
            MonsterData md = mapCfg.monsterLst[i];
            if (md.mWave == wave) {
                GameObject m = resSvc.LoadPrefab(md.mCfg.resPath, true);
                m.transform.localPosition = md.mBornPos;
                m.transform.localEulerAngles = md.mBornRote;
                m.transform.localScale = Vector3.one;
                m.name = "m" + md.mWave + "_" + md.mIndex;

                EntityMonster em = new EntityMonster {
                    battleMgr = this,
                    stateMgr = stateMgr,
                    skillMgr = skillMgr
                };

                // 设置初始属性
                em.md = md;
                em.SetBattleProps(md.mCfg.bps);
                em.Name = m.name;

                MonsterController mc = m.GetComponent<MonsterController>();
                mc.Init();
                em.SetController(mc);
                m.SetActive(false);
                monsterDic.Add(m.name, em);
                // 血条
                GameRoot.Instance.dynamicWnd.AddHpItemInfo(m.name, mc.hpRoot, em.HP);
            }
        }
    }

    /// <summary>
    /// 获取场景里所有的怪物实体
    /// </summary>
    public List<EntityMonster> GetEntityMonsters() {
        List<EntityMonster> monsterLst = new List<EntityMonster>();
        foreach (var item in monsterDic) {
            monsterLst.Add(item.Value);
        }
        return monsterLst;
    }

    /// <summary>
    /// 激活当前这一批次的怪物
    /// </summary>
    public void ActiveCurrentBatchMonsters() {
        TimerSvc.Instance.AddTimeTask((int tid) => {
            foreach (var item in monsterDic) {
                item.Value.SetActive();
                item.Value.Born();
                TimerSvc.Instance.AddTimeTask((int id) => {
                    // 出生 1 秒钟后进入Idle
                    item.Value.Idle();
                }, 1000);
            }
        }, 500);
    }

    #region 技能释放和角色控制
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

    public double lastAtkTime = 0; // 上一次攻击的时间
    private int[] comboArr = new int[] { 111, 112, 113, 114, 115 }; // 连招数组
    public int comboIndex = 0; // 当前连招的索引
    private void ReleaseNormalAtk() {
        //PECommon.Log("Click Normal Atk");
        // 开始连招
        if (entitySelfPlayer.currentAniState == AniState.Attack) {
            // 在 500ms 以内进行第二次点击，存数据到连招队列
            double nowAtkTime = TimerSvc.Instance.GetNowTime(); // 当前攻击的时间
            if (nowAtkTime - lastAtkTime < Constants.ComboSpace && lastAtkTime != 0) {
                if (comboArr[comboIndex] != comboArr[comboArr.Length - 1]) { // 越界检查
                    comboIndex += 1;
                    entitySelfPlayer.comboQue.Enqueue(comboArr[comboIndex]);
                    lastAtkTime = nowAtkTime;
                }
                else { // 重置连招
                    lastAtkTime = 0;
                    comboIndex = 0;
                }
            }
        }
        // 第一次进入
        else if (entitySelfPlayer.currentAniState == AniState.Idle || entitySelfPlayer.currentAniState == AniState.Move) {
            comboIndex = 0;
            lastAtkTime = TimerSvc.Instance.GetNowTime();
            entitySelfPlayer.Attack(comboArr[comboIndex]);
        }
    }

    private void ReleaseSkill1() {
        entitySelfPlayer.Attack(101);
    }

    private void ReleaseSkill2() {
        entitySelfPlayer.Attack(102);
    }

    private void ReleaseSkill3() {
        entitySelfPlayer.Attack(103);
    }

    public Vector2 GetDirInput() {
        return BattleSys.Instance.GetDirInput();
    }
    #endregion

    /// <summary>
    /// 移除怪物
    /// </summary>
    public void RmvMonster(string key) {
        EntityMonster entityMonster;
        if (monsterDic.TryGetValue(key, out entityMonster)) {
            monsterDic.Remove(key);
            GameRoot.Instance.dynamicWnd.RmvHpItemInfo(key); // 移除血条
        }
    }

}