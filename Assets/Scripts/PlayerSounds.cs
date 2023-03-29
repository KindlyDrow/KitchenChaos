using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{

    [SerializeField] private Player player;
    [SerializeField] private float footStepTimerMax;
    [SerializeField] private float volume = 1;
    private float footStepTimer;

    private void Update()
    {
        if (player.IsWalking())
        {
            footStepTimer -= Time.deltaTime;
            if(footStepTimer < 0)
            {
                footStepTimer = footStepTimerMax;
                SoundManager.Instance.PlayerStepsSound(player.transform.position, volume);
            }
        }
    }

}
