using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{

    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private bool waitingForPlayer;
    private void Start()
    {
        waitingForPlayer = true;
    }

    private void Update()
    {
        if (waitingForPlayer)
        {
            if (Player.LocalInstance != null)
            {
                Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
                waitingForPlayer = false;
            }
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArg e)
    {
        if(e.selectedCounter == baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    
    private void Hide()
    {
        foreach(GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }
        

    private void Show()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }
    }
}
