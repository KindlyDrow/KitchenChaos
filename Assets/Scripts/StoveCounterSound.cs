using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{

    [SerializeField] private StoveCounter stoveCounter;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SoundManager.Instance.OnSoundVolumeChange += SoundManager_OnSoundVolumeChange;
    }

    private void SoundManager_OnSoundVolumeChange(object sender, System.EventArgs e)
    {
        _audioSource.volume = SoundManager.Instance.globalSoundVolume;
    }

    public void PlaySound(bool play)
    {
        if (play)
        {
            _audioSource.Play();
        } else
        {
            _audioSource.Pause();
        }
    }

}
