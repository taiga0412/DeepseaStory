using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

//�ՓˊǗ�Component�B�Փ˂��N�������ۂɁA�C���X�y�N�^�Ŏw�肵�����\�b�h�i������Collider2D�j���Ăяo���B
[RequireComponent(typeof(Collider2D))]
public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private TriggerEvent onTriggerEnter = new TriggerEvent();
    [SerializeField] private TriggerEvent onTriggerStay = new TriggerEvent();

    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter.Invoke(other);
    }

    //Is Trigger��ON�ő���Collider�Əd�Ȃ��Ă���Ƃ��́A���̃��\�b�h����ɃR�[�������
    private void OnTriggerStay2D(Collider2D other)
    {
        //onTriggerStay�Ŏw�肳�ꂽ���������s����
        onTriggerStay.Invoke(other);
    }

    //UnityEvent���p�������N���X��[Serialize]������t�^���邱�ƂŁA
    //Inspector�E�B���h�E��ɕ\���ł���悤�ɂȂ�
    [Serializable]
    public class TriggerEvent : UnityEvent<Collider2D>
    {
    }
}
