using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public float masterVol, musicVol = 0f;

    public AudioMixer mainAudioMixer;

    private void Awake()
    {
        // destroy duplicates of this instance, only 1 at all times
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // functions that options menu calls
    public void SetMasterVolume(float newVolume)
    {
        masterVol = newVolume;
        mainAudioMixer.SetFloat("MasterVol", masterVol);
    }

    public void SetMusicVolume(float newVolume)
    {
        musicVol = newVolume;
        mainAudioMixer.SetFloat("MusicVol", musicVol);
    }
}
