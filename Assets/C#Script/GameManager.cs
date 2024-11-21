using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool gameIsCleared = false;

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
