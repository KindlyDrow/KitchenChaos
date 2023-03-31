using UnityEngine;

[CreateAssetMenu]
public class SoundListSO : ScriptableObject
{
    public AudioClip[] chops;
    public AudioClip[] deliveryFail;
    public AudioClip[] deliverySuccess;
    public AudioClip[] footsteps;
    public AudioClip[] objectDrop;
    public AudioClip[] objectPickup;
    public AudioClip[] trash;
    public AudioClip[] warning;
    
}
