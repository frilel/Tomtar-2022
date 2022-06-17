using UnityEngine;

class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } 
    }

    public void Play(Sound sound)
    {
        if (sound.Clip == null)
            return;

        if (sound.typeOfSound == TypeOfSound.Music)
        {
            musicSource.clip = sound.Clip;
            musicSource.Play();
        }
        else if (sound.typeOfSound == TypeOfSound.SFX)
        {
            sfxSource.clip = sound.Clip;
            sfxSource.Play();
        }
        else if (sound.typeOfSound == TypeOfSound.Voice)
        {
            voiceSource.clip = sound.Clip;
            voiceSource.Play();
        }
    }
}

public enum TypeOfSound
{
    Music,
    SFX,
    Voice
}