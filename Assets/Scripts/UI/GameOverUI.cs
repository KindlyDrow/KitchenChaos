using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    private void Start()
    {
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;

        Hide();
    }

    private void GameManager_OnStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance._state == GameManager.State.GameOver)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        textMeshProUGUI.text = DeliveryManager.Instance.successfulRecipesAmount.ToString();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
