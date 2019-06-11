/****************************************************
	文件：StateHit.cs
	作者：CaptainYun
	日期：2019/06/04 19:36   	
	功能：受击状态
*****************************************************/

using UnityEngine;

public class StateHit : IState {

    public void Enter(EntityBase entity, params object[] args) {
        entity.currentAniState = AniState.Hit;
        entity.RmvSkillCB();
    }

    public void Exit(EntityBase entity, params object[] args) {

    }

    public void Process(EntityBase entity, params object[] args) {
        if (entity.entityType == EntityType.Player) {
            entity.canRlsSkill = false;
        }

        entity.SetDir(Vector2.zero); // 中断移动
        entity.SetAction(Constants.ActionHit);

        // 受击音效
        if (entity.entityType == EntityType.Player) {
            AudioSource charAudio = entity.GetAudio();
            AudioSvc.Instance.PlayCharAudio(Constants.AssassinHit, charAudio);
        }

        TimerSvc.Instance.AddTimeTask((int tid) => {
            entity.SetAction(Constants.ActionDefault);
            entity.Idle();
        }, (int)(GetHitAniLen(entity) * 1000));
    }

    /// <summary>
    /// 受击的时间
    /// </summary>
    /// <param name="entity">受击实体</param>
    private float GetHitAniLen(EntityBase entity) {
        // 遍历指定实体所有的 Clip ，根据名字获得 Hit 动画的长度
        AnimationClip[] clips = entity.GetAniClips();
        for (int i = 0; i < clips.Length; i++) {
            string clipName = clips[i].name;
            if (clipName.Contains("hit") ||
                clipName.Contains("Hit") ||
                clipName.Contains("HIT")) {
                return clips[i].length;
            }
        }
        return 1; // 保护值，如果找不到
    }
}
