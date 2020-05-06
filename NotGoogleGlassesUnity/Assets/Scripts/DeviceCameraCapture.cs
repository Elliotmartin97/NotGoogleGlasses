using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class DeviceCameraCapture : MonoBehaviour
{
    private WebCamTexture cam_texture;
    private Texture2D preview;
    private Image image;
    private ODPluginCaller OD_plugin;
    public bool detect = false;
    public bool editormode = false;
    private int count = 0;
    private Material image_material;
    private Camera camera;
    private int preview_width = 640;
    private int preview_height = 480;

    private void Start()
    {
        OD_plugin = GameObject.Find("ObjectDetector").GetComponent<ODPluginCaller>();
        StartCamera();
        SetupImage();
        image_material = image.material;
        camera = OD_plugin.main_cam;
    }

    private void Update()
    {
        if (OD_plugin.GetJavaObjectState() || editormode)
        {
            UpdatePreview();
        }
    }

    private void UpdatePreview()
    {
        OD_plugin.GetLatestImageFromAndroid();
        ApplyImageToTexture();
        OD_plugin.GetDetectorResults();
        OD_plugin.ShowResults();
        camera.Render();
    }


    public void ApplyImageToTexture()
    {
        Color32[] colors = new Color32[preview_height * preview_width];

        for (int i = 0; i < preview_height * preview_width; i++)
        {
            colors[i].a = 255;
            colors[i].r = (byte)OD_plugin.GetRedData()[i];
            colors[i].g = (byte)OD_plugin.GetGreenData()[i];
            colors[i].b = (byte)OD_plugin.GetBlueData()[i];
        }
        count++;

        //preview.LoadRawTextureData(OD_plugin.GetRGBBytes());
        preview.SetPixels32(colors);
        preview.Apply();
        image.material.mainTexture = preview;
        //this updates the image to show the new texture, unsure why but it fixes the bug
        image.sprite = null;
        //manually render the camera
    }

    private void UpdateCamera()
    {
        if(cam_texture == null)
        {
            return;
        }
        preview.SetPixels(cam_texture.GetPixels());
        preview.Apply();
        image.material.mainTexture = preview;
    }

    private void StartCamera()
    {
        image = GetComponent<Image>();
        Debug.Log("Width = " + preview_width + " Height = " + preview_height);
        preview = new Texture2D(preview_width, preview_height, TextureFormat.RGB24, false);
        OD_plugin.CreateJavaObject();
        OD_plugin.InitJavaObject(preview_width, preview_height);
    }

    private void SetupImage()
    {
        if(image == null)
        {
            return;
        }
        RectTransform rect_transform = image.GetComponent<RectTransform>();

        float old_width = rect_transform.rect.width;
        float old_height = rect_transform.rect.height;
        //image.rectTransform.Rotate(Vector3.forward * 180);


    }



}
