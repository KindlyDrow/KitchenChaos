using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionMenu;

    [SerializeField] private Button resetButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button backButton;

    [SerializeField] private Button _selectedButton;

    private void Awake()
    {
        resetButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePouse();
        });
        restartButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            Loader.Load(Loader.Scene.GameScene);
        });
        menuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        optionButton.onClick.AddListener(() =>
        {
            ShowOptions();
        });
        backButton.onClick.AddListener(() =>
        {
            ShowMenu();
        });
    }

    private void OnEnable()
    {
        _selectedButton.Select();
    }

    private void Start()
    {
        GameManager.Instance.OnPause += GameManager_OnPause;
        Hide();
    }

    private void GameManager_OnPause(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.isPoused) Show(); else Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        ShowMenu();
        gameObject.SetActive(false);
    }

    private void ShowOptions()
    {
        HideMenu();
        optionMenu.SetActive(true);
    }

    private void HideOptions()
    {
        optionMenu.SetActive(false);
    }

    private void ShowMenu()
    {
        HideOptions();
        mainMenu.SetActive(true);
    }

    private void HideMenu()
    {
        mainMenu.SetActive(false);
    }
}
