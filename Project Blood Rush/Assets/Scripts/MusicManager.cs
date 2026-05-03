using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioSource musicAudioSource;
    public AudioClip[] tracks;
    private int currentTrackIndex = 0;
    public bool isPaused = false;

    void Awake()
    {
        // Singleton pattern to prevent duplicates
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (tracks.Length > 0 && !musicAudioSource.isPlaying)
        {
            PlayTrack(currentTrackIndex);
        }
    }

    void Update()
    {
        // if current song is ending go to next
        if (!musicAudioSource.isPlaying && !isPaused)
        {
            NextTrack();
        }
    }

    private void PlayTrack(int index)
    {
        if (tracks.Length == 0) return;

        musicAudioSource.clip = tracks[index];
        musicAudioSource.Play();
        isPaused = false;
    }

    public void NextTrack()
    {
        if (tracks.Length == 0) return;

        currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;
        PlayTrack(currentTrackIndex);
    }

    public void PreviousTrack()
    {
        if (tracks.Length == 0) return;

        currentTrackIndex = (currentTrackIndex - 1 + tracks.Length) % tracks.Length;
        PlayTrack(currentTrackIndex);
    }

    public void PauseTrack()
    {
        if (!isPaused)
        {
            musicAudioSource.Pause();
            isPaused = true;
          
        }
    }

    public void ResumeTrack()
    {
        if (isPaused)
        {
            musicAudioSource.UnPause();
            isPaused = false;
            
        }
    }
}