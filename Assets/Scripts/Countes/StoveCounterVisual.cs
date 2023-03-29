using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private GameObject[] stoveCounterVisuals;


    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        if (stoveCounterVisuals != null)
        {
            foreach (var item in stoveCounterVisuals)
            {
                item.SetActive(true);
            }
        }
    }

    public void Hide()
    {
        if (stoveCounterVisuals != null)
        {
            foreach (var item in stoveCounterVisuals)
            {
                item.SetActive(false);
            }
        }
    }




}
