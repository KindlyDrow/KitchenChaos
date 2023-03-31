using System;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource misicManager;

    public event EventHandler OnSoundVolumeChange;

    public static SoundManager Instance;

    [SerializeField] private SoundListSO soundListSO;

    private const string PLAYER_PREFS_GLOBAL_SOUND_VOLUME = "GlobalSoundVolume";
    private const string PLAYER_PREFS_GLOBAL_MUSIC_VOLUME = "GlobalMusicVolume";
    public float globalSoundVolume { get; private set; } = .8f;
    public float globalMusicVolume { get; private set; } = .2f;

    private void Awake()
    {
        globalMusicVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_GLOBAL_MUSIC_VOLUME, globalMusicVolume);
        globalSoundVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_GLOBAL_SOUND_VOLUME, globalSoundVolume);
        Instance = this;
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.OnPickedSomth += Player_OnPickedSomth;
        BaseCounter.OnAnyObjectPlaced += BaseCounter_OnAnyObjectPlaced;
        TrashCounter.OnSomthTrashed += TrashCounter_OnSomthTrashed;
        SetMusicVolume(globalMusicVolume);
        SetSoundVolume(globalSoundVolume);
    }

    private void TrashCounter_OnSomthTrashed(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(soundListSO.trash, trashCounter.transform.position);
    }

    private void BaseCounter_OnAnyObjectPlaced(object sender, System.EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(soundListSO.objectDrop, baseCounter.transform.position);
    }

    private void Player_OnPickedSomth(object sender, System.EventArgs e)
    {
        Player player = sender as Player;
        PlaySound(soundListSO.objectPickup, player.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(soundListSO.chops, cuttingCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
    {
        PlaySound(soundListSO.deliverySuccess, Vector3.zero);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
    {
        PlaySound(soundListSO.deliveryFail, Vector3.zero);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        int i = UnityEngine.Random.Range(0, audioClipArray.Count());
        AudioSource.PlayClipAtPoint(audioClipArray[i], position, volume * globalSoundVolume);
        
    }

    public void PlayCountdownSound()
    {
        PlaySound(soundListSO.warning, Vector3.zero);
    }

    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(soundListSO.warning, position);
    }


    public void PlayerStepsSound(Vector3 position, float volume = 1f)
    {
        int i = UnityEngine.Random.Range(0, soundListSO.footsteps.Count());
        AudioSource.PlayClipAtPoint(soundListSO.footsteps[i], position, volume * globalSoundVolume);
    }

    public void SetSoundVolume(float volume)
    {
        
        globalSoundVolume = volume;
        OnSoundVolumeChange?.Invoke(this, EventArgs.Empty);
        PlayerPrefs.SetFloat(PLAYER_PREFS_GLOBAL_SOUND_VOLUME, globalSoundVolume);
    }

    public void SetMusicVolume(float volume)
    {
        misicManager.volume = volume;
        globalMusicVolume = misicManager.volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_GLOBAL_MUSIC_VOLUME, globalMusicVolume);

    }
}
