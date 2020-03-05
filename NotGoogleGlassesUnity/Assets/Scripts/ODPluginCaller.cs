using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ODPluginCaller : MonoBehaviour
{
    private AndroidJavaObject detector_instance;
    private AndroidJavaObject activity_context;
    private List<RecognitionResult> current_results;
    private List<GameObject> detector_circles;
    private List<Text> texts;
    private GameObject canvas;
    public Text time_text;
    public Camera main_cam;
    private int[] red_data;
    private int[] green_data;
    private int[] blue_data;
    

    public class RecognitionResult
    {
        public string id;
        public string title;
        public float confidence;
        public Rect screen_position;
    }

    public int[] GetRedData()
    {
        return red_data;
    }

    public int[] GetGreenData()
    {
        return green_data;
    }

    public int[] GetBlueData()
    {
        return blue_data;
    }

    public void GetLatestImageFromAndroid()
    {
        red_data = detector_instance.Call<int[]>("GetRed");
        green_data = detector_instance.Call<int[]>("GetGreen");
        blue_data = detector_instance.Call<int[]>("GetBlue");
    }

    public void CreateJavaObject()
    {
        canvas = GameObject.Find("Canvas");
        detector_circles = new List<GameObject>();
        texts = new List<Text>();
        AndroidJavaClass unity_activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activity_context = unity_activity.GetStatic<AndroidJavaObject>("currentActivity");
        detector_instance = new AndroidJavaObject("org.tensorflow.lite.examples.detection.ObjectDetector");
    }

    public void InitJavaObject(int width, int height)
    {
        detector_instance.Call("Init", activity_context, width, height);
    }

    public bool GetJavaObjectState()
    {
        return detector_instance.Call<bool>("GetObjectState");
    }

    public void SendImageToDetector(Texture2D texture)
    {
        //Debug.Log("Unity Start");
        int width = texture.width;
        int height = texture.height;
        Color32[] pixels = texture.GetPixels32();
        int[] colorsARGB = new int[pixels.Length];
        for(int i = 0; i < pixels.Length; i++)
        {
            colorsARGB[i] = ColorToInt(pixels[i].a, pixels[i].r, pixels[i].g, pixels[i].b);
        }

        detector_instance.Call("CreateImageBitMap", width, height, colorsARGB);
        GetDetectorResults();
        ShowResults();
    }

    private static int ColorToInt(int alpha, int red, int green, int blue)
    {
        return (alpha << 24) | (red << 16) | (green << 8) | blue;
    }

    public static Color32 ConvertAndroidColor(int aCol)
    {
        byte[] values = BitConverter.GetBytes(aCol);
        if (!BitConverter.IsLittleEndian) Array.Reverse(values);

        Color32 c = new Color32(0, 0, 0, 255);
        c.r = values[2];
        c.g = values[1];
        c.b = values[0];

        //c.r = (byte)(aCol & 0xFF);
        //c.g = (byte)((aCol & 0xFF) >> 8);
        //c.b = (byte)((aCol & 0xFF) >> 16);
        return c;
    }

    


    public void GetDetectorResults()
    {
        current_results = new List<RecognitionResult>();
        int result_count = detector_instance.Call<int>("GetResultCount");
        string[] ids = detector_instance.Call<string[]>("GetResultIds");
        string[] titles = detector_instance.Call<string[]>("GetResultTitles");
        float[] confidences = detector_instance.Call<float[]>("GetResultConfidences");

        for(int i = 0; i < result_count; i++)
        {
            RecognitionResult new_result = new RecognitionResult();
            new_result.id = ids[i];
            new_result.title = titles[i];
            new_result.confidence = confidences[i];
            new_result.screen_position = GetRectFromFloats(detector_instance.Call<float[]>("GetResultPositionsByIndex", i));
            current_results.Add(new_result);
        }
    }

    private Rect GetRectFromFloats(float[] floats)
    {
        Rect rect = new Rect();
        rect.xMin = floats[2];
        rect.xMax = floats[3];
        rect.yMin = floats[0];
        rect.yMax = floats[1];
        rect.x = scaleRange(rect.x, 0, 300, -350, 350);
        rect.y = scaleRange(rect.y, 0, 300, -640, 640);
        return rect;
    }

    private float scaleRange(float number, float old_min, float old_max, float new_min, float new_max)
    {
        return (number / ((old_max - old_min) / (new_max - new_min))) + new_min;
    }

    public void ShowResults()
    {
        for (int i = 0; i < current_results.Count; i++)
        {
            if(texts.Count < current_results.Count)
            {
                GameObject obj = CreateDetectorCircle();
                obj.transform.position = new Vector2(obj.transform.position.x, 600.0f - (i * 100.0f));
                obj.transform.SetParent(canvas.transform.GetChild(1), false);
                texts.Add(obj.GetComponentInChildren<Text>());
                detector_circles.Add(obj);
            }
            if(current_results[i].confidence >= 0.5f)
            {
                detector_circles[i].SetActive(true);
            }
            else
            {
                detector_circles[i].SetActive(false);
            }
            texts[i].text = current_results[i].title + " " + current_results[i].confidence;
            detector_circles[i].transform.localPosition = new Vector2(current_results[i].screen_position.center.x, -current_results[i].screen_position.center.y);
            //time_text.text = "X = " + current_results[0].screen_position.center.x + " Y = " + -current_results[0].screen_position.center.y;
        }
    }

    private GameObject CreateDetectorCircle()
    {
        GameObject obj = Instantiate(Resources.Load("Prefabs/DetectorCircleTouch")) as GameObject;
        return obj;
    }

}
