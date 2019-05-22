/****************************************************
    文件：InfoWnd.cs
	作者：CaptainYun
    日期：2019/5/22 14:13:55
	功能：角色信息展示界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoWnd : WindowRoot {
    #region UI Define
    public Text txtInfo;
    public Text txtExp;
    public Image imgExpPrg;
    public Text txtPower;
    public Image imgPowerPrg;

    public Text txtJob;
    public Text txtFight;
    public Text txtHP;
    public Text txtHurt;
    public Text txtDef;

    public Button btnClose;

    public RawImage imgChar; // 角色展示
    #endregion

    protected override void InitWnd() {
        base.InitWnd();
        RegTouchEvts();
        RefreshUI();
    }

    private Vector2 startPos; // 按下的位置

    /// <summary>
    /// 触摸事件
    /// </summary>
    public void RegTouchEvts() {
        OnClickDown(imgChar.gameObject, (PointerEventData evt) => {
            startPos = evt.position;
            MainCitySys.Instance.SetStartRoate();
        });
        OnClickDrag(imgChar.gameObject, (PointerEventData evt) => {
            float roate = -(evt.position.x - startPos.x) * 0.4f; // 拖拽时要旋转的数据量
            MainCitySys.Instance.SetPlayerRoate(roate);
        });
    }

    public void ClickCloseBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        MainCitySys.Instance.CloseInfoWnd();
    }

    private void RefreshUI() {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtInfo, pd.name + " LV." + pd.lv);
        SetText(txtExp, pd.exp + "/" + PECommon.GetExpUpValByLv(pd.lv));
        imgExpPrg.fillAmount = pd.exp * 1.0F / PECommon.GetExpUpValByLv(pd.lv);
        SetText(txtPower, pd.power + "/" + PECommon.GetPowerLimit(pd.lv));
        imgPowerPrg.fillAmount = pd.power * 1.0F / PECommon.GetPowerLimit(pd.lv);

        SetText(txtJob, " 暗夜刺客");
        SetText(txtFight, PECommon.GetFightByProps(pd));
        SetText(txtHP, pd.hp);
        SetText(txtHurt, (pd.ad + pd.ap));
        SetText(txtDef, (pd.addef + pd.apdef));

        // 详细信息
        //SetText(dtxhp, pd.hp);
        //SetText(dtxad, pd.ad);
        //SetText(dtxap, pd.ap);
        //SetText(dtxaddef, pd.addef);
        //SetText(dtxapdef, pd.apdef);
        //SetText(dtxdodge, pd.dodge + "%");
        //SetText(dtxpierce, pd.pierce + "%");
        //SetText(dtxcritical, pd.critical + "%");
    }

    


}