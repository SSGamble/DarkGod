/****************************************************
	文件：MonsterController.cs
	作者：CaptainYun
	日期：2019/06/03 10:13   	
	功能：怪物表现实体角色控制器类
*****************************************************/

using UnityEngine;

public class MonsterController : Controller {

    private void Update() {
        // AI 逻辑表现
        if (isMove) {
            SetDir();
            SetMove();
        }
    }

    private void SetDir() {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    private void SetMove() {
        ctrl.Move(transform.forward * Time.deltaTime * Constants.MonsterMoveSpeed);
        // 修正美术资源问题：给一个向下的速度，便于在没有 apply root 时怪物可以落地
        ctrl.Move(Vector3.down * Time.deltaTime * Constants.MonsterMoveSpeed);
    }
}