// 7/29/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Level Music")]
    public AudioClip levelMusic;

    [Header("Area Music")]
    public AudioClip areaMusic;

    private void Awake()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Play the default level music at the start
        PlayLevelMusic();
    }

    /// <summary>
    /// Plays the default level music.
    /// </summary>
    public void PlayLevelMusic()
    {
        if (levelMusic != null)
        {
            audioSource.clip = levelMusic;
            audioSource.loop = true; // Loop the level music
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Level music is not assigned!");
        }
    }

    /// <summary>
    /// Plays the area-specific music.
    /// </summary>
    /// <param name="newAreaMusic">The AudioClip to play for the area.</param>
    public void PlayAreaMusic(AudioClip newAreaMusic)
    {
        if (newAreaMusic != null)
        {
            audioSource.clip = newAreaMusic;
            audioSource.loop = true; // Loop the area music
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Area music is not assigned!");
        }
    }
}
