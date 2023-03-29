using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{

    public enum Scene
    {
        MainMenuScene,
        GameScene,
        LoadingScene
    }

    public static string targetScene;

    public static void Load(Scene scene)
    {

        targetScene = scene.ToString();

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadTargetScene(String scene_name)
    {
        SceneManager.LoadScene(scene_name);
    }
}
