/****************************************************
    文件：MainCityWnd.cs
	作者：CaptainYun
    日期：2019/5/20 14:01:23
	功能：主城界面
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainCityWnd : WindowRoot {

    #region UIDefine
    public Animation menuAni;
    public Transform expPrgTrans;
    public Image imgPowerPrg;
    // 轮盘
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;

    public Text txtFight;
    public Text txtPower;
    public Text txtLv;
    public Text txtName;
    public Text txtExpPrg;

    public Button btnMenu;
    public Button btnGuide;

    private bool menuState = true; // 菜单的打开与关闭状态
    #endregion

    private AutoGuideCfg curtTaskData; // 当前引导任务 ID

    #region MainFunctions
    protected override void InitWnd() {
        base.InitWnd();
        pointDis = Screen.height * 1.0f / Constants.ScreenStanderHeight * Constants.ScreenOPDis; // 动态计算摇杆距离
        defPos = imgDirBg.transform.position; // 获取轮盘的默认位置
        SetActive(imgDirPoint, false); // 默认不显示轮盘点
        RegisterTouchEvts(); // 注册轮盘的触摸事件
        RefreshUI();
    }

    /// <summary>
    /// 刷新界面 UI
    /// </summary>
    public void RefreshUI() {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtFight, PECommon.GetFightByProps(pd));
        SetText(txtPower, pd.power + "/" + PECommon.GetPowerLimit(pd.lv));
        imgPowerPrg.fillAmount = pd.power * 1.0f / PECommon.GetPowerLimit(pd.lv);
        SetText(txtLv, pd.lv);
        SetText(txtName, pd.name);
        ExpPrg(pd);

        // 设置自动任务图比
        curtTaskData = resSvc.GetAutoGuideCfg(pd.guideid);
        if (curtTaskData != null) {
            SetGuideBtnIcon(curtTaskData.npcID);
        }
        else {
            SetGuideBtnIcon(-1);
        }
    }

    /// <summary>
    /// 更改任务引导的图标
    /// </summary>
    private void SetGuideBtnIcon(int npcID) {
        // 拿到对应的图片路径
        string spPath = "";
        Image img = btnGuide.GetComponent<Image>();
        switch (npcID) {
            case Constants.NPCWiseMan:
                spPath = PathDefine.WiseManHead;
                break;
            case Constants.NPCGeneral:
                spPath = PathDefine.GeneralHead;
                break;
            case Constants.NPCArtisan:
                spPath = PathDefine.ArtisanHead;
                break;
            case Constants.NPCTrader:
                spPath = PathDefine.TraderHead;
                break;
            default:
                spPath = PathDefine.TaskHead; // 默认图标
                break;
        }
        // 加载图片
        SetSprite(img, spPath);
    }

    /// <summary>
    /// 分段经验条屏幕适配和显示
    /// </summary>
    private void ExpPrg(PlayerData pd) {
        // 分段经验条屏幕适配
        GridLayoutGroup grid = expPrgTrans.GetComponent<GridLayoutGroup>();
        // 当前屏幕的缩放比例，因为是使用高度进行缩放的，所以，此处要使用高度进行计算
        float globalRate = 1.0f * Constants.ScreenStanderHeight / Screen.height;
        // 当前屏幕宽度
        float screenWidth = Screen.width * globalRate;
        // 当前经验块的宽度
        float width = (screenWidth - 177) / 10;
        grid.cellSize = new Vector2(width, 11);

        // 经验值
        int expPrgVal = (int)(pd.exp * 1.0f / PECommon.GetExpUpValByLv(pd.lv) * 100);
        SetText(txtExpPrg, expPrgVal + "%"); // 文本数值
        int index = expPrgVal / 10; // 经验条索引
        // 根据当前拥有的经验值，填充经验条
        for (int i = 0; i < expPrgTrans.childCount; i++) {
            Image img = expPrgTrans.GetChild(i).GetComponent<Image>();
            if (i < index) { // 前面都填充满
                img.fillAmount = 1;
            }
            else if (i == index) { // 数值位于这个经验块之间
                img.fillAmount = expPrgVal % 10 * 1.0f / 10;
            }
            else { // 后面都没满
                img.fillAmount = 0;
            }
        }
    }
    #endregion

    #region 点击事件
    /// <summary>
    /// 引导按钮，
    /// </summary>
    public void ClickGuideBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        if (curtTaskData!=null) {
            MainCitySys.Instance.RunTask(curtTaskData);
        }
        else {
            GameRoot.AddTips("引导任务已经完成");
        }
    }
     
    /// <summary>
    /// 主菜单按钮点击事件，播放相应的动画
    /// </summary>
    public void ClickMenuBtn() {
        audioSvc.PlayUIAudio(Constants.UIExtenBtn);
        menuState = !menuState;
        AnimationClip clip = null;
        if (menuState) {
            clip = menuAni.GetClip("OpenMCMenu");
        }
        else {
            clip = menuAni.GetClip("CloseMCMenu");
        }
        menuAni.Play(clip.name);
    }

    /// <summary>
    /// 点击头像，显示角色信息窗口
    /// </summary>
    public void ClickHeadBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenInfoWnd();
    }

    /// <summary>
    /// 强化
    /// </summary>
    public void ClickStrongBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenStrongWnd();
    }

    /// <summary>
    /// 聊天
    /// </summary>
    public void ClickChatBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenChatWnd();
    }

    /// <summary>
    /// 购买体力
    /// </summary>
    public void ClickBuyPowerBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenBuyWnd(0);
    }

    /// <summary>
    /// 铸造金币
    /// </summary>
    public void ClickMKCoinBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenBuyWnd(1);
    }

    /// <summary>
    /// 任务界面
    /// </summary>
    public void ClickTaskBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenTaskRewardWnd();
    }
    #endregion

    #region 摇杆
    private Vector2 startPos = Vector2.zero; // 按下轮盘的起始位置
    private Vector2 defPos = Vector2.zero; // 轮盘的默认位置
    private float pointDis; // 摇杆点标准距离

    /// <summary>
    /// 注册摇杆的各种点击拖拽事件
    /// </summary>
    public void RegisterTouchEvts() {
        // 按下
        OnClickDown(imgTouch.gameObject, (PointerEventData evt) => {
            startPos = evt.position;
            SetActive(imgDirPoint, true); // 激活轮盘点
            imgDirBg.transform.position = evt.position;
        });
        // 抬起
        OnClickUp(imgTouch.gameObject, (PointerEventData evt) => {
            imgDirBg.transform.position = defPos; // 恢复默认位置
            imgDirPoint.transform.localPosition = Vector2.zero; // 轮盘点归零
            SetActive(imgDirPoint, false); // 隐藏轮盘点
            // 方向信息传递，让角色停下来
            MainCitySys.Instance.SetMoveDir(Vector2.zero);
        });
        // 抬起
        OnClickDrag(imgTouch.gameObject, (PointerEventData evt) => {
            Vector2 dir = evt.position - startPos; // 方向信息
            float len = dir.magnitude; // 当前滑动的长度
            if (len > pointDis) { // 限制最远距离
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                imgDirPoint.transform.position = startPos + clampDir;
            }
            else {
                imgDirPoint.transform.position = evt.position;
            }
            // 传递方向信息，角色移动
            MainCitySys.Instance.SetMoveDir(dir.normalized);
        });
    }
    #endregion

}
