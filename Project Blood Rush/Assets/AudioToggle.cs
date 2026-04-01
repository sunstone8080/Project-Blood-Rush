using UnityEngine;

public class AudioToggle : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // get the AudioSource on this GameObject
        if (audioSource == null)
            Debug.LogWarning("No AudioSource found on " + gameObject.name);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && audioSource != null)
        {
            audioSource.enabled = !audioSource.enabled; // toggle on/off
        }
    }
}