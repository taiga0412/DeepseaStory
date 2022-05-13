using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//�V�[�����Ǘ�����N���X�B
//�Q�[���I�[�o�[��X�e�[�W�N���A�̃V�[���J�ڂ��Ǘ�����B
public class MainSceneController : SingletonMonoBehaviorInScene<MainSceneController>
{
    //�Q�[���I�[�o�[�̑J��
    public void GameOver()
    {
        StartCoroutine(SceneChange("TestScene", 3.0f));
    }

    //�X�e�[�W�N���A�̑J��
    public void StageClear()
    {
        //SceneChange("StageSelectScene", 3.0f);
    }

    //�w��b����ɃV�[����؂�ւ���
    private IEnumerator SceneChange(string name, float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        SceneManager.LoadScene(name);
    }
}
