using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー用のステータスComponent
public class PlayerStatus : MobStatus
{
    public override void OnDie()
    {
        base.OnDie();
        MainSceneController.Instance.GameOver();    //死亡時にゲームオーバー遷移を呼び出す
    }
}
