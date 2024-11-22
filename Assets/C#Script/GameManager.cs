using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] GameObject player;

    private bool gameIsCleared = false;

    private string currentInput = ""; // 入力履歴を保存
    private string secretCommand = "tanaka"; // 隠しコマンド
    private bool isSecretCommand = false;

    private void Start()
    {
        Time.timeScale = 1;  // ゲームを通常速度に戻す
    }

    //隠しコマンド
    private void Update()
    {
        // キー入力をチェック
        if (Input.GetKey(KeyCode.T)) currentInput += "t";
        if (Input.GetKeyDown(KeyCode.N)) currentInput += "n";
        if (Input.GetKeyDown(KeyCode.A)) currentInput += "a";
        if (Input.GetKeyDown(KeyCode.K)) currentInput += "k";

        // 入力履歴の長さを制限（コマンド以上の文字は削除）
        if (currentInput.Length > secretCommand.Length)
        {
            currentInput = currentInput.Substring(currentInput.Length - secretCommand.Length);
        }

        // コマンドが一致するか確認
        if (currentInput == secretCommand)
        {
            ActivateSecret();
        }
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

    private void ActivateSecret()
    {
        if (!isSecretCommand)
        {
            isSecretCommand = true;
            PlayerControl playerCO = player.GetComponent<PlayerControl>();
            playerCO.bulletCount = 10;
            playerCO.shieldObject.SetActive(true);
            playerCO.isShield = true;
        }
        
    }
}
