using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�V�[����Ɉ�������݂��Ȃ��I�u�W�F�N�g�̒��ۃN���X�B
//�p������ƁA�N���X��.Instance�ŎQ�Ɖ\�ɂȂ�B
public abstract class SingletonMonoBehaviorInScene<T> : MonoBehaviour where T :MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            //�V�[����2�ȏ�C���X�^���X�����݂���ꍇ�́A�G���[���o��
            throw new Exception("�V�[�����ɑ��̃C���X�^���X�����݂��܂��I");
        }

        Instance = this as T;
    }
}
