/****************************************************
    文件：ItemEnityHp.cs
	作者：CaptainYun
    日期：2019/6/4 20:31:4
	功能：血条物体
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class ItemEntityHP : MonoBehaviour {

    #region UI Define
    public Image imgHPYellow;
    public Image imgHPRed;

    public Animation criticalAni;
    public Text txtCritical;

    public Animation dodgeAni;
    public Text txtDodge;

    public Animation hpAni;
    public Text txtHp;
    #endregion

    private Transform rootTrans; // 怪物的 Trans
    private RectTransform rect;
    private int hpVal; // hp 数值
    private float scaleRate = 1.0F * Constants.ScreenStandardHeight / Screen.height; // 屏幕缩放比例

    private void Update() {
        /*
        if (Input.GetKeyUp(KeyCode.Space)) {
            SetCritical(696);
            SetHurt(336);
        }

        if (Input.GetKeyUp(KeyCode.A)) {
            SetDodge();
        }
        */
        Vector3 screenPos = Camera.main.WorldToScreenPoint(rootTrans.position); // 3维坐标转屏幕坐标
        rect.anchoredPosition = screenPos * scaleRate;

        UpdateMixBlend();
        imgHPYellow.fillAmount = currentPrg; // 黄血条渐变
    }

    public void InitItemInfo(Transform trans, int hp) {
        rect = transform.GetComponent<RectTransform>();
        rootTrans = trans;
        hpVal = hp;
        imgHPYellow.fillAmount = 1;
        imgHPRed.fillAmount = 1;
    }

    /// <summary>
    /// 黄血条渐变动画混合
    /// </summary>
    private void UpdateMixBlend() {
        if (Mathf.Abs(currentPrg - targetPrg) < Constants.AccelerHPSpeed * Time.deltaTime) {
            currentPrg = targetPrg;
        }
        else if (currentPrg > targetPrg) {
            currentPrg -= Constants.AccelerHPSpeed * Time.deltaTime;
        }
        else {
            currentPrg += Constants.AccelerHPSpeed * Time.deltaTime;
        }
    }

    public void SetCritical(int critical) {
        criticalAni.Stop(); // 停止没播放完的动画，新动画直接覆盖就行了
        txtCritical.text = "暴击 " + critical;
        criticalAni.Play();
    }

    public void SetDodge() {
        dodgeAni.Stop();
        txtDodge.text = "闪避";
        dodgeAni.Play();
    }

    public void SetHurt(int hurt) {
        hpAni.Stop();
        txtHp.text = "-" + hurt;
        hpAni.Play();
    }

    private float currentPrg; // 血条当前进度，黄条
    private float targetPrg; // 血条目标进度，红条

    public void SetHPVal(int oldVal, int newVal) {
        currentPrg = oldVal * 1.0f / hpVal;
        targetPrg = newVal * 1.0f / hpVal;
        imgHPRed.fillAmount = targetPrg;
    }
}