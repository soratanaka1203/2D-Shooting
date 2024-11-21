using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool gameIsCleared = false;

    public void OnGameClear()
    {
        gameIsCleared = true;  // �Q�[���N���A�t���O�𗧂Ă�
        // �Q�[���N���A��̏���
        StopGame();
    }

    private void StopGame()
    {
        // �Q�[���̐i�s���~
        Time.timeScale = 0;  // �Q�[�����ꎞ��~
    }
}
