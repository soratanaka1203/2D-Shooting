
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] GameObject itemCanvas;
    [SerializeField] GameObject defaultCanvas;

    //スタートボタンが押されたとき画面を遷移させる
    public void TapStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("InGameScene");
    }

    //ランキングボタンが押されたとき画面を遷移させる
    public void TapRanking()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("RankingScene");
    }

    //操作説明ボタンが押されたとき画面を遷移させる
    public void TapOperationExplanation()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("OperationExplanationScene");
    }

    public void BackTitle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }

    public void LoadThisScene() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ActiveItemCanvas()
    {
        defaultCanvas.SetActive(false);
        itemCanvas.SetActive(true);
    }

    public void BackDefaultCanvas()
    {
        defaultCanvas.SetActive(true);
        itemCanvas.SetActive(false);
    }
}
