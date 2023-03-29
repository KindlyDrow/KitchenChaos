using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{

    private const string CUT = "Cut";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPlayerStartCutting()
    {
        animator.SetTrigger(CUT);
    }
}
