using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource sfxSource;

    [Header("SFX")]
    public AudioClip buttonClick;

    [Header("Tower SFX")]
    public AudioClip blockPlace;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (!PlayerData.Instance.SoundOn || clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayButton()
    {
        if (buttonClick != null)
            PlaySFX(buttonClick);
    }

    public void PlayBlockPlace()
    {
        if (blockPlace != null)
            PlaySFX(blockPlace);
    }
}
