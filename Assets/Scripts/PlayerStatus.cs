using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�v���C���[�p�̃X�e�[�^�XComponent
public class PlayerStatus : MobStatus
{
    public override void OnDie()
    {
        base.OnDie();
        MainSceneController.Instance.GameOver();    //���S���ɃQ�[���I�[�o�[�J�ڂ��Ăяo��
    }
}
