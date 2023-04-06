using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _readyText;
    [SerializeField] private TextMeshProUGUI _notReadyText;

    private void Start()
    {
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReady;
        Show();
        ShowNotReady();
    }

    private void GameManager_OnLocalPlayerReady(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady())
        {
            ShowReady();
        } else
        {
            ShowNotReady();
        }
    }

    private void GameManager_OnStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameCoundownToStart())
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowReady()
    {
        _readyText.gameObject.SetActive(true);
        _notReadyText.gameObject.SetActive(false);
    }

    private void ShowNotReady()
    {
        _readyText.gameObject.SetActive(false);
        _notReadyText.gameObject.SetActive(true);
    }
}
