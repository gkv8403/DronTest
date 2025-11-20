using UnityEngine;

public class DroneCameraSwitcher : MonoBehaviour
{
    public Camera mainCam;
    public Camera povCam;
    public Camera topCam;

    private int camIndex = 0;

    void Start()
    {
        SetCamera(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            camIndex++;
            if (camIndex > 2) camIndex = 0;

            SetCamera(camIndex);
        }
    }

    void SetCamera(int index)
    {
        // Keep ALL cameras enabled always
        mainCam.enabled = true;
        povCam.enabled = true;
        topCam.enabled = true;

        // Reset depth for all
        mainCam.depth = 0;
        povCam.depth = 0;
        topCam.depth = 0;

        // Give active camera highest priority
        if (index == 0) mainCam.depth = 10;
        if (index == 1) povCam.depth = 10;
        if (index == 2) topCam.depth = 10;
    }
}
