using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectDatabase : MonoBehaviour
{
    public List<GameObject> data_objects;


    private void Awake()
    {
        GameObject[] obj_list = Resources.LoadAll<GameObject>("Prefabs/DataBaseObjects/");
        for(int i = 0; i < obj_list.Length; i++)
        {
            data_objects.Add(obj_list[i]);
        }
    }

    public string GetInfoByName(string value)
    {
        for(int i = 0; i < data_objects.Count; i++)
        {
            if(value == data_objects[i].GetComponent<DataObject>().title)
            {
                return data_objects[i].GetComponent<DataObject>().info;
            }
        }
        return "No Information";
    }

}
