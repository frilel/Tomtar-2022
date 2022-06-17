using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip Clip;
    public TypeOfSound typeOfSound;

    [Range(0f, 1f)] public float Volume;
    [Range(.1f, 3f)] public float Pitch;

}