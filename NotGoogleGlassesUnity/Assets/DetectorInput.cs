using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectorInput : MonoBehaviour
{
    public string title;
    public string info;
    public GameObject object_information;


    public void Touched()
    {
        if(object_information == null)
        {
            object_information = GameObject.Find("Canvas").transform.GetChild(2).gameObject;

        }

        object_information.SetActive(true);
        object_information.transform.GetChild(0).GetComponent<Text>().text = title;
        object_information.transform.GetChild(1).GetComponent<Text>().text = info;
    }
}
