using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool gameIsCleared = false;

    private void Start()
    {
        Time.timeScale = 1;  // ゲームを通常速度に戻す
    }

    public void OnGameClear()
    {
        gameIsCleared = true;  // ゲームクリアフラグを立てる
        // ゲームクリア後の処理
        StopGame();
    }

    private void StopGame()
    {
        // ゲームの進行を停止
        Time.timeScale = 0;  // ゲームを一時停止
    }
}
