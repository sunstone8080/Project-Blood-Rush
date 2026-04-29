using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider masterVolSlider;
    public Slider musicVolSlider;

    private void Start()
    {
        // set slider values to current settings on start
        if (SettingsManager.Instance != null)
        {
            masterVolSlider.SetValueWithoutNotify(SettingsManager.Instance.masterVol);
            musicVolSlider.SetValueWithoutNotify(SettingsManager.Instance.musicVol);
        }
    }

    // update volume value dynamically with sliders
    public void UpdateMasterVolume(float value)
    {
        SettingsManager.Instance.SetMasterVolume(value);
    }

    public void UpdateMusicVolume(float value)
    {
        SettingsManager.Instance.SetMusicVolume(value);
    }
}
