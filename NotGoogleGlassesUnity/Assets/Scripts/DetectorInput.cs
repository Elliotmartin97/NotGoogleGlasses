using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectorInput : MonoBehaviour
{
    public string title;
    public GameObject object_information;
    public GameObject guesser;
    public bool guessing = false;

    public void Touched()
    {
        if(object_information == null)
        {
            object_information = GameObject.Find("Canvas").transform.GetChild(2).gameObject;

        }
        if(guesser == null)
        {
            guesser = GameObject.Find("Canvas").transform.GetChild(3).gameObject;
        }

        if(guessing)
        {
            guesser.SetActive(true);
            guesser.GetComponent<GuesserScript>().answer = title;
            guesser.GetComponent<GuesserScript>().Active();
            object_information.transform.GetChild(0).GetComponent<Text>().text = title;
            object_information.transform.GetChild(1).GetComponent<Text>().text = object_information.GetComponent<ObjectDatabase>().GetInfoByName(title);
        }
        else
        {
            object_information.SetActive(true);
            object_information.transform.GetChild(0).GetComponent<Text>().text = title;
            object_information.transform.GetChild(1).GetComponent<Text>().text = object_information.GetComponent<ObjectDatabase>().GetInfoByName(title);
        }
    }
}
