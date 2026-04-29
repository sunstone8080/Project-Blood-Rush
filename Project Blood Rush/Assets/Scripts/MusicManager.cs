using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioClip[] tracks;
    private int currentTrackIndex = 0;
    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        if (tracks.Length > 0)
        {
            PlayTrack(currentTrackIndex);
        }
    }

    // Update is called once per frame
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
        musicAudioSource.clip = tracks[index];
        musicAudioSource.Play();
        isPaused = false;
    }

    public void NextTrack()
    {
        // loop through tracks with modulus, play next
        currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;
        PlayTrack(currentTrackIndex);
    }

    public void PreviousTrack()
    {
        // loop through tracks with modulus, play previous
        currentTrackIndex = (currentTrackIndex - 1 + tracks.Length) % tracks.Length;
        PlayTrack(currentTrackIndex);
    }

    public void PauseTrack()
    {
        if(!isPaused)
        {
            musicAudioSource.Pause();
            isPaused = true;
        }
    }

    public void ResumeTrack()
    {
        if(isPaused)
        {
            musicAudioSource.UnPause();
            isPaused = false;
        }
    }
}
