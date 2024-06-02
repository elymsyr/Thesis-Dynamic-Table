using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;
    public Camera camera3;
    public Camera camera4;
    public Camera camera5;

    void Start()
    {
        EnableCamera(camera1);
        DisableCamera(camera2);
        DisableCamera(camera3);
        DisableCamera(camera4);
        DisableCamera(camera5);
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
            DisableCamera(camera3);
            DisableCamera(camera4);
            DisableCamera(camera5);
        }
        else if (camera2.enabled)
        {
            EnableCamera(camera3);
            DisableCamera(camera1);
            DisableCamera(camera2); 
            DisableCamera(camera4); 
            DisableCamera(camera5); 
        }
        else if (camera3.enabled)
        {
            EnableCamera(camera4);
            DisableCamera(camera2);
            DisableCamera(camera3);
            DisableCamera(camera1);
            DisableCamera(camera5);
        }
        else if (camera4.enabled)
        {
            EnableCamera(camera5);
            DisableCamera(camera2);
            DisableCamera(camera3);
            DisableCamera(camera1);
            DisableCamera(camera4);
        }
        else
        {
            EnableCamera(camera1);
            DisableCamera(camera2);
            DisableCamera(camera3);
            DisableCamera(camera4);
            DisableCamera(camera5);
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
