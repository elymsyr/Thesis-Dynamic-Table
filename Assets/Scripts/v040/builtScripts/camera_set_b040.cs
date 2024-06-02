using UnityEngine;

public class CameraSwitcherB040 : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    void Start()
    {
        EnableCamera(camera1);
        DisableCamera(camera2);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleCameras();
        }
    }

    void ToggleCameras()
    {
        if (camera1.enabled)
        {
            EnableCamera(camera2);
            DisableCamera(camera1);
        }
        else
        {
            EnableCamera(camera1);
            DisableCamera(camera2);
        }
    }

    void EnableCamera(Camera cam)
    {
        cam.enabled = true;
    }

    void DisableCamera(Camera cam)
    {
        cam.enabled = false;
    }
}
