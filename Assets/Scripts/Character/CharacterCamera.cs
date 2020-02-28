using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : CharacterBase
{
    [System.Serializable]
    public struct CameraSettings
    {
        public float zoomDistance;
        public float minRotation;
        public float maxRotation;
        public float rotateX;
        public float rotateY;
    }

    public CameraSettings cameraSettings = new CameraSettings()
    {
        zoomDistance = 5.0f,
        minRotation = -90.0f,
        maxRotation = 90.0f,
        rotateX = 5.0f,
        rotateY = 5.0f
    };

    protected Vector3 position;

    protected float camX;
    protected float camY;

    private int zoom;

    public int Zoom
    {
        get
        {
            return (zoom);
        }
    }

    public void ZoomIn()
    {
        zoom--;
        if (zoom < 0)
        {
            zoom = 0;
        }
    }

    public void ZoomOut()
    {
        zoom++;
    }

    public bool FPS
    {
        get
        {
            return (Zoom == 0);
        }
    }

    public bool TPS
    {
        get
        {
            return (Zoom == 1);
        }
    }

    public bool Shooter
    {
        get
        {
            return (Zoom <= 1);
        }
    }

    public float ZoomDistance
    {
        get
        {
            return (zoom * cameraSettings.zoomDistance);
        }
    }

    private new Camera camera;

    public Camera Camera
    {
        get
        {
            if (camera == null)
            {
                camera = GetComponentInChildren<Camera>();
                if (camera == null)
                {
                    camera = new GameObject("camera").AddComponent<Camera>();
                    camera.transform.SetParent(transform);
                }
            }
            return (camera);
        }
    }

    public void RotateCamera(float x, float y)
    {
        camX = Mathf.Clamp(camX + x, cameraSettings.minRotation, cameraSettings.maxRotation);
        camY += y;

        if (Shooter)
        {
            position = Vector3.up * (Body.Height - Body.Radius) + new Vector3(0.0f, Mathf.Sin(Mathf.Deg2Rad * camX), -Mathf.Cos(Mathf.Deg2Rad * camX)) * ZoomDistance;
            Camera.transform.localPosition = position;
            Camera.transform.localEulerAngles = Vector3.right * camX;
            camY = Camera.transform.rotation.eulerAngles.y;
        }
        else
        {
            position = transform.position + Vector3.up * Body.Height + Quaternion.Euler(camX, camY, 0.0f) * Vector3.back * ZoomDistance;
            Camera.transform.position = position;
            Camera.transform.LookAt(transform.position + Vector3.up * Body.Height);
        }
    }
}