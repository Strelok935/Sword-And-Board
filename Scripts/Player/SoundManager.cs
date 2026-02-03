// 7/29/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    [Range(0.1f, 5f)]
    private float crossfadeDuration = 0.75f;

    private readonly System.Collections.Generic.HashSet<AudioClip> cachedClips = new();
    private AudioSource primarySource;
    private AudioSource secondarySource;
    private AudioSource activeSource;

    [Header("Level Music")]
    public AudioClip levelMusic;

    [Header("Area Music")]
    public AudioClip areaMusic;

    private void Awake()
    {
        primarySource = GetComponent<AudioSource>();
        if (primarySource == null)
        {
            primarySource = gameObject.AddComponent<AudioSource>();
        }

        secondarySource = gameObject.AddComponent<AudioSource>();
        CopyAudioSourceSettings(primarySource, secondarySource);

        activeSource = primarySource;
    }

    private void Start()
    {
        PreloadClip(levelMusic);
        PlayLevelMusic();
    }

    /// <summary>
    /// Plays the default level music.
    /// </summary>
    public void PlayLevelMusic()
    {
        PlayMusicClip(levelMusic, "Level music is not assigned!");
    }

    /// <summary>
    /// Plays the area-specific music.
    /// </summary>
    /// <param name="newAreaMusic">The AudioClip to play for the area.</param>
    public void PlayAreaMusic(AudioClip newAreaMusic)
    {
        PlayMusicClip(newAreaMusic, "Area music is not assigned!");
    }

    private void PlayMusicClip(AudioClip clip, string missingWarning)
    {
        if (clip == null)
        {
            Debug.LogWarning(missingWarning);
            return;
        }

        PreloadClip(clip);

        if (activeSource.clip == clip && activeSource.isPlaying)
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(CrossfadeToClip(clip));
    }

    private void PreloadClip(AudioClip clip)
    {
        if (clip == null || cachedClips.Contains(clip))
        {
            return;
        }

        cachedClips.Add(clip);
        if (clip.loadState == AudioDataLoadState.Unloaded)
        {
            clip.LoadAudioData();
        }
    }

    private System.Collections.IEnumerator CrossfadeToClip(AudioClip nextClip)
    {
        AudioSource nextSource = activeSource == primarySource ? secondarySource : primarySource;
        nextSource.clip = nextClip;
        nextSource.loop = true;
        nextSource.volume = 0f;
        nextSource.Play();

        float elapsed = 0f;
        float startVolume = activeSource.volume;

        while (elapsed < crossfadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / crossfadeDuration);
            activeSource.volume = Mathf.Lerp(startVolume, 0f, t);
            nextSource.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        activeSource.Stop();
        activeSource.volume = startVolume;
        activeSource = nextSource;
    }

    private static void CopyAudioSourceSettings(AudioSource source, AudioSource target)
    {
        target.outputAudioMixerGroup = source.outputAudioMixerGroup;
        target.bypassEffects = source.bypassEffects;
        target.bypassListenerEffects = source.bypassListenerEffects;
        target.bypassReverbZones = source.bypassReverbZones;
        target.playOnAwake = false;
        target.priority = source.priority;
        target.volume = source.volume;
        target.pitch = source.pitch;
        target.panStereo = source.panStereo;
        target.spatialBlend = source.spatialBlend;
        target.reverbZoneMix = source.reverbZoneMix;
        target.dopplerLevel = source.dopplerLevel;
        target.spread = source.spread;
        target.rolloffMode = source.rolloffMode;
        target.minDistance = source.minDistance;
        target.maxDistance = source.maxDistance;
    }
}
