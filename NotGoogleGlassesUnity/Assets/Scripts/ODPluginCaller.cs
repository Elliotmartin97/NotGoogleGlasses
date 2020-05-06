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
    private RecognitionResult test_result;
    private List<GameObject> detector_objects;
    private List<GameObject> line_objects;
    private GameObject canvas;
    public Camera main_cam;
    private int[] red_data;
    private int[] green_data;
    private int[] blue_data;
    private byte[] rgb_bytes;
    public int current_detector = 0;
    public int current_mode = 0;

    public GuesserScript guesser;


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

    public byte[] GetRGBBytes()
    {
        return rgb_bytes;
    }


    public void GetLatestImageFromAndroid()
    {
        red_data = detector_instance.Call<int[]>("GetRed");
        green_data = detector_instance.Call<int[]>("GetGreen");
        blue_data = detector_instance.Call<int[]>("GetBlue");
        //rgb_bytes = detector_instance.Call<byte[]>("GetRGBBytes");
    }

    public void CreateJavaObject()
    {
        canvas = GameObject.Find("Canvas");
        detector_objects = new List<GameObject>();
        line_objects = new List<GameObject>();
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
        rect.xMin = floats[0];
        rect.xMax = floats[1];
        rect.yMin = -floats[2];
        rect.yMax = -floats[3];

        return rect;
    }

    public Vector2 GetScaledPosition(Vector2 original)
    {
        return new Vector2(MapLerp(-300, 300, 0, 640, original.x), MapLerp(-210, 210, -480, 0, original.y));
    }

    private float MapLerp(float min, float max, float old_min, float old_max, float value)
    {
        return Mathf.Lerp(min, max, Mathf.InverseLerp(old_min, old_max, value));
    }

    private float scaleRange(float number, float old_min, float old_max, float new_min, float new_max)
    {
        return (number / ((old_max - old_min) / (new_max - new_min))) + new_min;
    }

    public void ShowResults()
    {
        switch(current_detector)
        {
            case 0:
                DetectSingles();
                break;
            case 1:
                DetectRects();
                break;
            case 2:
                DetectMultiples();
                break;
        }
    }

    private void DetectSingles()
    {
        for (int i = 0; i < current_results.Count; i++)
        {
            if (detector_objects.Count < current_results.Count)
            {
                GameObject obj = CreateDetectorCircle();
                obj.transform.SetParent(canvas.transform.GetChild(1), false);
                detector_objects.Add(obj);
            }
            if (current_results[i].confidence >= 0.55f)
            {
                detector_objects[i].SetActive(true);
            }
            else
            {
                detector_objects[i].SetActive(false);
            }
            detector_objects[i].transform.localPosition = GetScaledPosition(current_results[i].screen_position.center);
            detector_objects[i].GetComponent<DetectorInput>().title = current_results[i].title;
            ShowObjectTitles(detector_objects[i].transform.GetChild(0).GetComponent<Text>(), current_results[i].title);
        }
    }

    private void DetectRects()
    {
        for (int i = 0; i < current_results.Count; i++)
        {
            if (detector_objects.Count < current_results.Count)
            {
                GameObject obj = CreateDetectorRect();
                obj.transform.SetParent(canvas.transform.GetChild(1), false);
                detector_objects.Add(obj);
            }
            if (current_results[i].confidence >= 0.55f)
            {
                detector_objects[i].SetActive(true);
            }
            else
            {
                detector_objects[i].SetActive(false);
            }
            detector_objects[i].transform.localPosition = GetScaledPosition(current_results[i].screen_position.center);
            detector_objects[i].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(Mathf.Abs(current_results[i].screen_position.width), Mathf.Abs(current_results[i].screen_position.height));

            detector_objects[i].GetComponent<DetectorInput>().title = current_results[i].title;
            ShowObjectTitles(detector_objects[i].transform.GetChild(0).GetComponent<Text>(), current_results[i].title);
        }
    }

    private void DetectMultiples()
    {
        for(int i = 0; i < detector_objects.Count; i++)
        {
            Destroy(detector_objects[i]);
        }
        detector_objects = new List<GameObject>();
        for(int i = 0; i < current_results.Count; i++)
        {
            if(current_results[i].confidence <= 0.54f)
            {
                continue;
            }
            if(detector_objects.Count < 1)
            {
                GameObject obj = CreateDetectorMultiple();
                obj.GetComponentInChildren<Text>().text = current_results[i].title;
                obj.name = current_results[i].title;
                obj.transform.SetParent(canvas.transform.GetChild(1), false);
                obj.transform.localPosition = Vector2.zero;
                detector_objects.Add(obj);
                continue;
            }

            for(int j = 0; j < detector_objects.Count; j++)
            {
                bool new_result = true;
                if(current_results[i].title == detector_objects[j].name)
                {
                    new_result = false;
                }
                
                if(new_result == true)
                {
                    GameObject obj = CreateDetectorMultiple();
                    obj.GetComponentInChildren<Text>().text = current_results[i].title;
                    obj.name = current_results[i].title;
                    obj.transform.SetParent(canvas.transform.GetChild(1), false);
                    obj.transform.localPosition = Vector2.zero;
                    detector_objects.Add(obj);
                    break;
                }

            }
        }

        for(int i = 0; i < detector_objects.Count; i++)
        {
            int count = 0;
            Vector2 position = Vector2.zero;
            List<Vector2> detection_positions = new List<Vector2>();
            for(int j = 0; j < line_objects.Count; j++)
            {
                Destroy(line_objects[j]);
            }
            line_objects = new List<GameObject>();
            for(int j = 0; j < current_results.Count; j++)
            {
                if(current_results[j].confidence <= 0.54f)
                {
                    continue;
                }
                if(current_results[j].title == detector_objects[i].name)
                {
                    count++;
                    GameObject obj = CreateLine();
                    obj.transform.SetParent(detector_objects[i].transform, false);
                    Vector2 pos = transform.InverseTransformPoint(GetScaledPosition(current_results[j].screen_position.center));
                    detection_positions.Add(pos);
                    line_objects.Add(obj);
                    position += pos;

                }
            }
            position /= count;
            detector_objects[i].transform.localPosition = position;
            for (int j = 0; j < detection_positions.Count; j++)
            {
                Vector2 difference = (Vector2)line_objects[j].transform.localPosition - detection_positions[j];
                RectTransform rect_transform = line_objects[j].GetComponent<RectTransform>();
                rect_transform.sizeDelta = new Vector2(difference.magnitude, 5.0f);
                rect_transform.pivot = new Vector2(0.0f, 0.5f);
                rect_transform.localPosition = detection_positions[j];
                float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                rect_transform.rotation = Quaternion.Euler(0, 0, angle);


            }
            ShowObjectTitles(detector_objects[i].GetComponentInChildren<Text>(), detector_objects[i].name); 
        }

    }

    private void ShowObjectTitles(Text text_obj, string title)
    {
        if(current_mode == 0)
        {
            if(guesser.CheckStringWithAnsweredTitles(title) == true)
            {
                text_obj.text = title;
                text_obj.color = Color.green;
                text_obj.transform.parent.GetComponent<DetectorInput>().guessing = false;
            }
            else
            {
                text_obj.text = "?";
                text_obj.color = Color.yellow;
                text_obj.transform.parent.GetComponent<DetectorInput>().guessing = true;
            }
        }
        else if(current_mode == 1)
        {
            text_obj.text = title;
            text_obj.color = Color.black;
            text_obj.transform.parent.GetComponent<DetectorInput>().guessing = false;
        }
    }

    private GameObject CreateDetectorCircle()
    {
        GameObject obj = Instantiate(Resources.Load("Prefabs/DetectorCircleTouch")) as GameObject;
        return obj;
    }

    private GameObject CreateDetectorRect()

    {
        GameObject obj = Instantiate(Resources.Load("Prefabs/DetectorRectTouch")) as GameObject;
        return obj;
    }

    private GameObject CreateDetectorMultiple()
    {
        GameObject obj = Instantiate(Resources.Load("Prefabs/DetectorMultiple")) as GameObject;
        return obj;
    }

    private GameObject CreateLine()
    {
        GameObject obj = Instantiate(Resources.Load("Prefabs/ImageLine")) as GameObject;
        return obj;
    }

    public void SetLineRendererPositions(LineRenderer line, Rect rect)
    {
        float min_x = 0 - rect.min.x;
        float min_y = 0 - rect.min.y;
        float max_x = 0 + rect.max.x;
        float max_y = 0 + rect.max.y;

        line.SetPosition(0, new Vector2(0, 0));
        line.SetPosition(1, new Vector2(20, 0));

    }


    public void SwitchDetector(int index)
    {
        for(int i = 0; i < detector_objects.Count; i++)
        {
            Destroy(detector_objects[i]);
        }
        detector_objects = new List<GameObject>();
        current_detector = index;
    }

    public void SwitchMode(int index)
    {
        for (int i = 0; i < detector_objects.Count; i++)
        {
            Destroy(detector_objects[i]);
        }
        detector_objects = new List<GameObject>();
        current_mode = index;

    }

}
