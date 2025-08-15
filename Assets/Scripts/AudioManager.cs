using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource[] audioSources;

    private void Awake()
    {
        if(instance == null)
            instance = this;

        audioSources = GetComponentsInChildren<AudioSource>();
    }

    public AudioSource GetFreeAudioSource()
    {
        foreach (var source in audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        return null;
    }

    public void PlaySound(AudioClip clip, float volume = 0.2f)
    {
        AudioSource audioSource = GetFreeAudioSource();

        if (audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }
}
