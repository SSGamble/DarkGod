/****************************************************
    文件：AudioSvc.cs
	作者：CaptainYun
    日期：2019/5/13 21:31:16
	功能：声音播放服务
*****************************************************/

using UnityEngine;

public class AudioSvc : MonoBehaviour {
    // 单例
    public static AudioSvc Instance = null;

    public AudioSource bgAudio;
    public AudioSource uiAudio;

    public void InitSvc() {
        Instance = this;
        PECommon.Log("Init AudioSvc...");
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name">音乐名</param>
    /// <param name="isLoop">是否循环播放</param>
    public void PlayBGMusic(string name, bool isLoop = true) {
        AudioClip ac = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        // 当前没有音乐或音乐不一样
        if (bgAudio.clip == null || bgAudio.clip.name != ac.name) {
            bgAudio.clip = ac; // 设置当前音乐
            bgAudio.loop = isLoop;
            bgAudio.Play();
        }
    }

    /// <summary>
    /// 播放 UI 操作音效
    /// </summary>
    /// <param name="name">音效名</param>
    public void PlayUIAudio(string name) {
        AudioClip ac = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        uiAudio.clip = ac; // 设置当前音乐
        uiAudio.Play();
    }

}