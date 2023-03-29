using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    

    private void Awake()
    {
        playButton.onClick.AddListener(OnClickPlayButton);
        quitButton.onClick.AddListener(OnClickQuitButton);


    }

    private void Start()
    {
    }

    private void OnClickPlayButton()
    {
        Loader.Load(Loader.Scene.GameScene);
    }

    private void OnClickQuitButton()
    {
        Application.Quit();
    }
}
