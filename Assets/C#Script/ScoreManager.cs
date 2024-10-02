using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace MyNameSpace
{
    public class ScoreManager : MonoBehaviour
    {
        // シングルトンインスタンス
        public static ScoreManager Instance { get; private set; }

        // スコアを保持するプロパティ
        public int score { get; private set; }
        public static int[] ranking = new int[4] { 0, 0, 0, 0 };
        [SerializeField] List<TextMeshProUGUI> rankingText;

        // 初期化
        private void Awake()
        {
            // 他にインスタンスがない場合、現在のインスタンスを設定
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // このオブジェクトをシーンを跨いで破棄しない
            }
            else
            {
                // 既にインスタンスが存在する場合は、現在のオブジェクトを破棄
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            // シーンがロードされたときのイベント登録
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            // イベント解除
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // スコアを加算するメソッド
        public void AddScore(int points)
        {
            score += points;
        }

        // スコアをリセットするメソッド
        public void ResetScore()
        {
            score = 0;
        }

        // スコアの表示をするメソッド
        public void SetDisplayScore(TextMeshProUGUI scoreText)
        {
            scoreText.text = "スコア：" + score;
        }

        // ランキングにスコアを設定
        public void SetRanking(int score)
        {
            for (int i = 0; i < ranking.Length; i++)
            {
                if (ranking[i] <= score)
                {
                    // スコアを挿入し、既存のスコアをシフト
                    int temp = ranking[i];
                    ranking[i] = score;

                    // 下位のランキングをシフトさせる
                    for (int j = i + 1; j < ranking.Length; j++)
                    {
                        int nextTemp = ranking[j];
                        ranking[j] = temp;
                        temp = nextTemp;
                    }
                    break; // 新しいスコアを挿入したらループを終了
                }
            }
        }


        // シーンがロードされたときに呼び出されるメソッド
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 現在のシーンがランキングのシーンならテキストに反映させる
            if (scene.name == "RankingScene")
            {
                rankingText = new List<TextMeshProUGUI>();

                for (int i = 0; i < ranking.Length; i++)
                {
                    var textObject = GameObject.Find("ranking" + (i + 1)); // ランキングテキストを参照

                    if (textObject != null)
                    {
                        TextMeshProUGUI textMeshPro = textObject.GetComponent<TextMeshProUGUI>();
                        if (textMeshPro != null)
                        {
                            // ランキングに設定
                            rankingText.Add(textMeshPro);
                            textMeshPro.text = (i + 1) + "位：" + ranking[i];
                            Debug.Log(textMeshPro.text);
                        }
                        else
                        {
                            Debug.LogError("TextMeshProUGUI component not found on ranking" + (i + 1));
                        }
                    }
                    else
                    {
                        Debug.LogError("Ranking text object ranking" + (i + 1) + " not found.");
                    }
                }
            }
        }
    }
}
