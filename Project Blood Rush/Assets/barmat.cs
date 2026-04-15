using UnityEngine;

public class BarMat : MonoBehaviour
{
    [Header("Bar Points")]
    public Transform standPoint;
    public Transform cameraPoint;

    private void Awake()
    {
        // Optional auto-assign if named correctly
        if (standPoint == null)
            standPoint = transform.Find("BarStandPoint");

        if (cameraPoint == null)
            cameraPoint = transform.Find("BarCameraPoint");
    }
}