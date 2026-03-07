namespace NovelCore.Runtime.Core.AudioSystem;

/// <summary>
/// Interface for audio playback services.
/// Provides wrapper around Unity's audio system for extensibility.
/// </summary>
public interface IAudioService
{
    /// <summary>
    /// Plays background music with optional fade in.
    /// </summary>
    /// <param name="musicClip">Audio clip to play.</param>
    /// <param name="loop">Whether to loop the music.</param>
    /// <param name="fadeInDuration">Fade in duration in seconds (0 for instant).</param>
    void PlayMusic(AudioClip musicClip, bool loop = true, float fadeInDuration = 0f);

    /// <summary>
    /// Stops currently playing music with optional fade out.
    /// </summary>
    /// <param name="fadeOutDuration">Fade out duration in seconds (0 for instant).</param>
    void StopMusic(float fadeOutDuration = 0f);

    /// <summary>
    /// Plays a sound effect.
    /// </summary>
    /// <param name="sfxClip">Sound effect clip to play.</param>
    /// <param name="volume">Volume multiplier (0-1).</param>
    void PlaySFX(AudioClip sfxClip, float volume = 1f);

    /// <summary>
    /// Sets the master volume for all audio.
    /// </summary>
    /// <param name="volume">Volume level (0-1).</param>
    void SetMasterVolume(float volume);

    /// <summary>
    /// Sets the music volume.
    /// </summary>
    /// <param name="volume">Volume level (0-1).</param>
    void SetMusicVolume(float volume);

    /// <summary>
    /// Sets the sound effects volume.
    /// </summary>
    /// <param name="volume">Volume level (0-1).</param>
    void SetSFXVolume(float volume);

    /// <summary>
    /// Gets the current music volume.
    /// </summary>
    float MusicVolume { get; }

    /// <summary>
    /// Gets the current SFX volume.
    /// </summary>
    float SFXVolume { get; }

    /// <summary>
    /// Checks if music is currently playing.
    /// </summary>
    bool IsMusicPlaying { get; }

    /// <summary>
    /// Pauses all audio.
    /// </summary>
    void PauseAll();

    /// <summary>
    /// Resumes all audio.
    /// </summary>
    void ResumeAll();
}
