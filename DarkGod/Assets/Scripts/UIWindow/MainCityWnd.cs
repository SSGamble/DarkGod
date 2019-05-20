/****************************************************
    �ļ���LoginWnd.cs
	���ߣ�CaptainYun
    ���ڣ�2019/5/20 14:01:2
	���ܣ����� UI ����
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
    /// ˢ�½��� UI
    /// </summary>
    public void RefreshUI() {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtFight, PECommon.GetFightByProps(pd));
        SetText(txtPower, "������" + pd.power + "/" + PECommon.GetPowerLimit(pd));
        imgPowerPrg.fillAmount = pd.power * 1.0f / PECommon.GetPowerLimit(pd);
        SetText(txtLv, pd.lv);
        SetText(txtName, pd.name);

        // �ֶξ�������Ļ����
        GridLayoutGroup grid = expPrgTrans.GetComponent<GridLayoutGroup>();
        // ��ǰ�����ű�������Ϊ��ǰ�� UI �����ǻ��ڸ߶ȵģ������ڴ˴�ʹ�ø߶ȼ������
        float globalRate = 1.0f * Constants.ScreenStanderHeight / Screen.height;
        // ��ǰ UI ��ʵ�Ŀ��
        float screenWidth = Screen.width * globalRate;
        // ��ǰ�����Ŀ��
        float width = (screenWidth - 177) / 10;
        grid.cellSize = new Vector2(width, 11);
    }

    private void Update() {
        RefreshUI();
    }
}
