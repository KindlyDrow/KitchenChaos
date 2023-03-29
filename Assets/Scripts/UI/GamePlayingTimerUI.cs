using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingTimerUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    private void Start()
    {
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;

        Hide();
    }

    private void GameManager_OnStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance._state == GameManager.State.GamePlaying)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Update()
    {
        float fillAmount = 1 - GameManager.Instance.gamePlayingTimer / GameManager.Instance.gamePlayingTimerMax;
        timerImage.fillAmount = fillAmount;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
