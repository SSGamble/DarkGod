/****************************************************
    文件：TaskWnd.cs
	作者：CaptainYun
    日期：2019/5/31 21:33:30
	功能：任务奖励界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TaskWnd : WindowRoot {
    public Transform scrollTrans; // 内容的滑动列表
    private PlayerData pd = null;
    private List<TaskRewardData> trdLst = new List<TaskRewardData>();

    protected override void InitWnd() {
        base.InitWnd();

        pd = GameRoot.Instance.PlayerData;
        RefreshUI();
    }

    public void RefreshUI() {
        trdLst.Clear();

        List<TaskRewardData> todoLst = new List<TaskRewardData>(); // 没有领取的
        List<TaskRewardData> doneLst = new List<TaskRewardData>(); // 已经完成的

        //1|0|0
        for (int i = 0; i < pd.taskArr.Length; i++) {
            string[] taskInfo = pd.taskArr[i].Split('|');
            TaskRewardData trd = new TaskRewardData {
                id = int.Parse(taskInfo[0]),
                prgs = int.Parse(taskInfo[1]),
                taked = taskInfo[2].Equals("1")
            };

            if (trd.taked) { // 已被领取
                doneLst.Add(trd);
            }
            else { // 没领取
                todoLst.Add(trd);
            }
        }

        // 这样有了一个分类排序的效果
        trdLst.AddRange(todoLst);
        trdLst.AddRange(doneLst);

        // 先删除所有任务项
        for (int i = 0; i < scrollTrans.childCount; i++) {
            Destroy(scrollTrans.GetChild(i).gameObject);
        }

        // 实例化 任务奖励 Item Prefab
        for (int i = 0; i < trdLst.Count; i++) {
            GameObject go = resSvc.LoadPrefab(PathDefine.TaskItemPrefab);
            go.transform.SetParent(scrollTrans);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.name = "taskItem_" + i;

            TaskRewardData trd = trdLst[i];
            TaskRewardCfg trf = resSvc.GetTaskRewardCfg(trd.id);

            // 设置 Item 的具体显示
            SetText(GetTrans(go.transform, "txtName"), trf.taskName);
            SetText(GetTrans(go.transform, "txtPrg"), trd.prgs + "/" + trf.count);
            SetText(GetTrans(go.transform, "txtExp"), "奖励：        经验" + trf.exp);
            SetText(GetTrans(go.transform, "txtCoin"), "金币" + trf.coin);
            Image imgPrg = GetTrans(go.transform, "prgBar/prgVal").GetComponent<Image>();
            float prgVal = trd.prgs * 1.0f / trf.count;
            imgPrg.fillAmount = prgVal;

            Button btnTake = GetTrans(go.transform, "btnTake").GetComponent<Button>();
            //btnTake.onClick.AddListener(ClickTakeBtn);
            btnTake.onClick.AddListener(() => {
                ClickTakeBtn(go.name);
            });

            Transform transComp = GetTrans(go.transform, "imgComp");
            Image imgIcon = btnTake.GetComponent<Image>(); // 宝箱图标
            if (trd.taked) { // 被领取了
                imgIcon.color = new Color32(167, 167, 167, 140);
                btnTake.interactable = false;
                SetActive(transComp);
            }
            else {
                SetActive(transComp, false);
                if (trd.prgs == trf.count) { // 已达条件，但还未领取
                    btnTake.interactable = true;
                    SetSprite(imgIcon, PathDefine.TaskRewardIcon);
                    imgIcon.color = new Color32(255, 255, 255, 255);
                }
                else { // 没到条件
                    btnTake.interactable = false;
                    imgIcon.color = new Color32(255, 255, 255, 255);
                }
            }

        }
    }

    private void ClickTakeBtn(string name) {
        PECommon.Log("点击");
        string[] nameArr = name.Split('_'); // go.name = "taskItem_" + i;
        int index = int.Parse(nameArr[1]); // 点击了哪一个

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.ReqTakeTaskReward,
            reqTakeTaskReward = new ReqTakeTaskReward {
                rid = trdLst[index].id
            }
        };
        netSvc.SendMsg(msg);

        TaskRewardCfg trc = resSvc.GetTaskRewardCfg(trdLst[index].id);
        int coin = trc.coin;
        int exp = trc.exp;
        GameRoot.AddTips(Constants.Color("获得奖励：", TxtColor.Blue) + Constants.Color(" 金币 +" + coin + " 经验 +" + exp, TxtColor.Green));
    }

    public void ClickCloseBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetWndState(false);
    }
}