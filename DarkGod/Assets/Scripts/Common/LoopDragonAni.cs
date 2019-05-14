/****************************************************
    文件：LoopDragonAni.cs
	作者：CaptainYun
    日期：2019/5/13 22:11:3
	功能：飞龙循环动画
*****************************************************/

using UnityEngine;

/// <summary>
/// 因为默认动画，飞龙只会绕着画面飞一圈，所以需要循环播放动画，让龙一圈又一圈的飞
/// </summary>
public class LoopDragonAni : MonoBehaviour {
    private Animation ani;

    private void Awake() {
        ani = transform.GetComponent<Animation>();
    }

    private void Start() {
        if (ani != null) {
            // 每 18s，重复播放动画
            InvokeRepeating("PlayDragonAni", 0, 18);
        }
    }

    /// <summary>
    /// 播放飞龙动画
    /// </summary>
    private void PlayDragonAni() {
        if (ani != null) {
            ani.Play();
        }
    }
}