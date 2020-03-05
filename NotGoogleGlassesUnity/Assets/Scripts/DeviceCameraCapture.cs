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
    private bool do_once = false;
    public Text text;
    private int count = 0;

    private void Start()
    {
        OD_plugin = GameObject.Find("ObjectDetector").GetComponent<ODPluginCaller>();
        StartCamera();
        SetupImage();
    }

    private void Update()
    {
        if (OD_plugin.GetJavaObjectState() || editormode)
        {
            UpdatePreview();
        }

        //UpdateCamera();
    }

    private void UpdatePreview()
    {
        OD_plugin.GetLatestImageFromAndroid();
        ApplyImageToTexture();
        OD_plugin.GetDetectorResults();
        OD_plugin.ShowResults();
    }


    public void ApplyImageToTexture()
    {
        Color32[] colors = new Color32[1080 * 1920];
        for(int i = 0; i < 1080 * 1920; i++)
        {
            colors[i].a = 255;
            colors[i].r = (byte)OD_plugin.GetRedData()[i];
            colors[i].g = (byte)OD_plugin.GetGreenData()[i];
            colors[i].b = (byte)OD_plugin.GetBlueData()[i];
        }
        count++;
        preview.SetPixels32(colors);
        preview.Apply();
        image.material.mainTexture = preview;
        text.text = count.ToString();
        //this updates the image to show the new texture, unsure why but it fixes the bug
        image.sprite = null;
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
        int width = 1920;
        int height = 1080;
        Debug.Log("Width = " + width + " Height = " + height);
        preview = new Texture2D(width, height);
        OD_plugin.CreateJavaObject();
        OD_plugin.InitJavaObject(width, height);
    }

    private void SetupImage()
    {
        if(image == null)
        {
            return;
        }
        RectTransform rect_transform = image.GetComponent<RectTransform>();
        rect_transform.sizeDelta = new Vector2(1080, 2220);


        float old_width = rect_transform.rect.width;
        float old_height = rect_transform.rect.height;
        image.rectTransform.Rotate(Vector3.forward * 270);
        rect_transform.sizeDelta = new Vector2(old_height, old_width);

        
    }



}
