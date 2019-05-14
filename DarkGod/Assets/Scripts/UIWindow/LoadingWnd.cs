/****************************************************
    文件：LoadingWnd.cs
	作者：CaptainYun
    日期：2019/5/12 22:37:42
	功能：加载进度界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LoadingWnd : WindowRoot {

    public Text txtPrg; // 进度百分比
    public Text txtTips;
    public Image imgFG;
    public Image imgPoint;

    private float fgWidth; // 前景进度条的宽度

    /// <summary>
    /// 初始化窗口信息
    /// </summary>
    protected override  void InitWnd() {
        base.InitWnd();
        fgWidth = imgFG.GetComponent<RectTransform>().sizeDelta.x; //获取前景进度条的宽度
        SetText(txtTips, "这是一条游戏信息");
        SetText(txtPrg, "0%");
        imgFG.fillAmount = 0; // 进度条填充 0，（0-1）
        imgPoint.transform.localPosition = new Vector3(-(fgWidth/2), 0, 0); // 进度光标的位置，进度条的最左边，即进度条宽度的一半的左边
    }

    /// <summary>
    /// 设置进度
    /// </summary>
    /// <param name="prg">(0-1) 之间的数</param>
    public void SetProgress(float prg) {
        SetText(txtPrg, (int)(prg * 100) + "%"); // 设置进度数值
        imgFG.fillAmount = prg; // 填充进度条

        // 设置进度点的位置
        float posX = prg * fgWidth - (fgWidth / 2);
        imgPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0);
    }
}