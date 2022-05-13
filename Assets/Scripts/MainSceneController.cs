using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//シーンを管理するクラス。
//ゲームオーバーやステージクリアのシーン遷移を管理する。
public class MainSceneController : SingletonMonoBehaviorInScene<MainSceneController>
{
    //ゲームオーバーの遷移
    public void GameOver()
    {
        StartCoroutine(SceneChange("TestScene", 3.0f));
    }

    //ステージクリアの遷移
    public void StageClear()
    {
        //SceneChange("StageSelectScene", 3.0f);
    }

    //指定秒数後にシーンを切り替える
    private IEnumerator SceneChange(string name, float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        SceneManager.LoadScene(name);
    }
}
