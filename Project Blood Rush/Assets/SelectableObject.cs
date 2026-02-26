using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    private Renderer objRenderer;
    private Color originalColor;

    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        originalColor = objRenderer.material.color;
    }

    // Called when player looks at object
    public void OnHover()
    {
        objRenderer.material.color = Color.yellow;
    }

    // Called when player looks away
    public void OnUnhover()
    {
        objRenderer.material.color = originalColor;
    }

    // Called when player presses interact
    public void OnInteract()
    {
        Debug.Log("Interacted with " + gameObject.name);
        // Add your interaction logic here
    }
}
