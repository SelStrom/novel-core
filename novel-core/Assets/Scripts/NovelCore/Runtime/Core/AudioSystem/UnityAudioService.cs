using System.Collections;

namespace NovelCore.Runtime.Core.AudioSystem
{

/// <summary>
/// Unity implementation of IAudioService.
/// Manages music and sound effects using Unity's AudioSource components.
/// </summary>
public class UnityAudioService : MonoBehaviour, IAudioService
{
    [Header("Audio Sources")]
    [SerializeField]
    [Tooltip("AudioSource for background music")]
    private AudioSource _musicSource;

    [SerializeField]
    [Tooltip("AudioSource for sound effects")]
    private AudioSource _sfxSource;

    [Header("Volume Settings")]
    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("Master volume multiplier")]
    private float _masterVolume = 1f;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("Music volume")]
    private float _musicVolume = 0.7f;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("SFX volume")]
    private float _sfxVolume = 1f;

    private Coroutine _musicFadeCoroutine;

    // Properties
    public float MusicVolume => _musicVolume;
    public float SFXVolume => _sfxVolume;
    public bool IsMusicPlaying => _musicSource != null && _musicSource.isPlaying;

    private void Awake()
    {
        // Create audio sources if not assigned
        if (_musicSource == null)
        {
            var musicObject = new GameObject("MusicSource");
            musicObject.transform.SetParent(transform);
            _musicSource = musicObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
        }

        if (_sfxSource == null)
        {
            var sfxObject = new GameObject("SFXSource");
            sfxObject.transform.SetParent(transform);
            _sfxSource = sfxObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.loop = false;
        }

        UpdateVolumes();
        Debug.Log("UnityAudioService: Initialized");
    }

    public void PlayMusic(AudioClip musicClip, bool loop = true, float fadeInDuration = 0f)
    {
        if (musicClip == null)
        {
            Debug.LogWarning("UnityAudioService: Cannot play null music clip");
            return;
        }

        // Stop any ongoing fade
        if (_musicFadeCoroutine != null)
        {
            StopCoroutine(_musicFadeCoroutine);
            _musicFadeCoroutine = null;
        }

        _musicSource.loop = loop;
        _musicSource.clip = musicClip;

        if (fadeInDuration > 0f)
        {
            _musicFadeCoroutine = StartCoroutine(FadeInMusic(fadeInDuration));
        }
        else
        {
            _musicSource.volume = _musicVolume * _masterVolume;
            _musicSource.Play();
        }

        Debug.Log($"UnityAudioService: Playing music {musicClip.name}");
    }

    public void StopMusic(float fadeOutDuration = 0f)
    {
        if (!_musicSource.isPlaying)
        {
            return;
        }

        // Stop any ongoing fade
        if (_musicFadeCoroutine != null)
        {
            StopCoroutine(_musicFadeCoroutine);
            _musicFadeCoroutine = null;
        }

        if (fadeOutDuration > 0f)
        {
            _musicFadeCoroutine = StartCoroutine(FadeOutMusic(fadeOutDuration));
        }
        else
        {
            _musicSource.Stop();
        }

        Debug.Log("UnityAudioService: Music stopped");
    }

    public void PlaySFX(AudioClip sfxClip, float volume = 1f)
    {
        if (sfxClip == null)
        {
            Debug.LogWarning("UnityAudioService: Cannot play null SFX clip");
            return;
        }

        var adjustedVolume = Mathf.Clamp01(volume) * _sfxVolume * _masterVolume;
        _sfxSource.PlayOneShot(sfxClip, adjustedVolume);

        Debug.Log($"UnityAudioService: Playing SFX {sfxClip.name}");
    }

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        Debug.Log($"UnityAudioService: Master volume set to {_masterVolume}");
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        Debug.Log($"UnityAudioService: Music volume set to {_musicVolume}");
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        Debug.Log($"UnityAudioService: SFX volume set to {_sfxVolume}");
    }

    public void PauseAll()
    {
        _musicSource.Pause();
        _sfxSource.Pause();
        Debug.Log("UnityAudioService: All audio paused");
    }

    public void ResumeAll()
    {
        _musicSource.UnPause();
        _sfxSource.UnPause();
        Debug.Log("UnityAudioService: All audio resumed");
    }

    private void UpdateVolumes()
    {
        if (_musicSource != null)
        {
            _musicSource.volume = _musicVolume * _masterVolume;
        }

        if (_sfxSource != null)
        {
            _sfxSource.volume = _sfxVolume * _masterVolume;
        }
    }

    private IEnumerator FadeInMusic(float duration)
    {
        _musicSource.volume = 0f;
        _musicSource.Play();

        var targetVolume = _musicVolume * _masterVolume;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            yield return null;
        }

        _musicSource.volume = targetVolume;
        _musicFadeCoroutine = null;
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        var startVolume = _musicSource.volume;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        _musicSource.Stop();
        _musicSource.volume = _musicVolume * _masterVolume;
        _musicFadeCoroutine = null;
    }
}

}