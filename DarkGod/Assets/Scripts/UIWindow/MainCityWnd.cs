/****************************************************
    文件：LoginWnd.cs
	作者：CaptainYun
    日期：2019/5/20 14:01:2
	功能：主城 UI 界面
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class MainCityWnd : WindowRoot {

    public Text txtFight;
    public Text txtPower;
    public Image imgPowerPrg;
    public Text txtLv;
    public Text txtName;
    public Text txtExpPrg;
    public Transform expPrgTrans;

    protected override void InitWnd() {
        base.InitWnd();
        RefreshUI();
    }

    /// <summary>
    /// 刷新界面 UI
    /// </summary>
    public void RefreshUI() {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtFight, PECommon.GetFightByProps(pd));
        SetText(txtPower, "体力：" + pd.power + "/" + PECommon.GetPowerLimit(pd));
        imgPowerPrg.fillAmount = pd.power * 1.0f / PECommon.GetPowerLimit(pd);
        SetText(txtLv, pd.lv);
        SetText(txtName, pd.name);

        // 分段经验条屏幕适配
        GridLayoutGroup grid = expPrgTrans.GetComponent<GridLayoutGroup>();
        // 当前的缩放比例，因为当前的 UI 缩放是基于高度的，所以在此处使用高度计算比例
        float globalRate = 1.0f * Constants.ScreenStanderHeight / Screen.height;
        // 当前 UI 真实的宽度
        float screenWidth = Screen.width * globalRate;
        // 当前经验块的宽度
        float width = (screenWidth - 177) / 10;
        grid.cellSize = new Vector2(width, 11);
    }

    private void Update() {
        RefreshUI();
    }
}
