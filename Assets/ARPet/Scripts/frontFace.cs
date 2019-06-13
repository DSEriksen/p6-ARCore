using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class frontFace : MonoBehaviour
{

    private bool cameraAvailable;
    private WebCamTexture frontcam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    void Start()
    {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if(devices.Length == 0)
        {
            Debug.Log("no devices were found");
            cameraAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if(devices[i].isFrontFacing)
            {
                frontcam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if(frontcam == null)
        {
            Debug.Log("unable to find front facing camera");
            return;
        }

        frontcam.Play();
        background.texture = frontcam;
        cameraAvailable = true;

    }


    void Update()
    {
        if (!cameraAvailable)
            return;


        float ratio = (float)frontcam.width / (float)frontcam.height;
        fit.aspectRatio = ratio;

        float scaleY = frontcam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);


        int orient = -frontcam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

    }
}
