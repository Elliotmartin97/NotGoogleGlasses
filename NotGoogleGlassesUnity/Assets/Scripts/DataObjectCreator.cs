using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;



public class DataObjectCreator : MonoBehaviour
{
    private List<GameObject> view_list;
    public InputField title_input;
    public InputField info_input;
    private GameObject current_object;
    public GameObject object_editor;

    private void Awake()
    {
        view_list = new List<GameObject>();
        CreateDatabaseList();
    }

    public void CreateDatabaseList()
    {
        if(view_list.Count > 0)
        {
            for(int i = 0; i < view_list.Count; i++)
            {
                Destroy(view_list[i]);
            }
            view_list = null;
        }
        view_list = new List<GameObject>();
        GameObject[] obj_list = Resources.LoadAll<GameObject>("Prefabs/DatabaseObjects/");
        for(int i = 0; i < obj_list.Length; i++)
        {
            
            view_list.Add(CreateDataView(i, obj_list[i].GetComponent<DataObject>()));
        }
    }

    private GameObject CreateDataView(int index, DataObject data)
    {
        GameObject view_object = Instantiate(Resources.Load<GameObject>("Prefabs/ObjectView")) as GameObject;
        view_object.GetComponent<Button>().onClick.AddListener(() => LoadObject(view_object));
        view_object.GetComponent<DataObject>().title = data.title;
        view_object.GetComponent<DataObject>().info = data.info;
        view_object.GetComponentInChildren<Text>().text = data.title;
        view_object.transform.SetParent(GameObject.Find("Container").transform, false);
        return view_object;
    }

    public void LoadObject(GameObject obj)
    {
        object_editor.SetActive(true);
        for(int i = 0; i < view_list.Count; i++)
        {
            if(obj == view_list[i])
            {
                current_object = Instantiate(Resources.Load<GameObject>("Prefabs/DataObjectPrefab"));

                DataObject obj_data = current_object.GetComponent<DataObject>();
                DataObject view_data = view_list[i].GetComponent<DataObject>();
                current_object.name = view_data.title;
                obj_data.title = view_data.title;
                obj_data.info = view_data.info;
                title_input.text = view_data.title;
                info_input.text = view_data.info;
            }
        }
    }

    public void CreateNewObject()
    {
        current_object = Instantiate(Resources.Load<GameObject>("Prefabs/DataObjectPrefab"));
    }

    public void CreateOrEditDataObject()
    {
#if UNITY_EDITOR
        if (!current_object)
        {
            Debug.Log("No Object");
            return;
        }
        DataObject obj_data = current_object.GetComponent<DataObject>();
        obj_data.title = title_input.text;
        obj_data.info = info_input.text;

        //check existing object to delete
        GameObject[] obj_list = Resources.LoadAll<GameObject>("Prefabs/DatabaseObjects/");
        for (int i = 0; i < obj_list.Length; i++)
        {
            if (obj_list[i].name == current_object.name)
            {
                File.Delete("Assets/Resources/Prefabs/DatabaseObjects/" + obj_list[i].name + ".prefab");
                //should delete the existing asset
            }
        }

        //create new object
        current_object.name = obj_data.title;
        PrefabUtility.SaveAsPrefabAsset(current_object, "Assets/Resources/Prefabs/DatabaseObjects/" + current_object.name + ".prefab");
#endif
    }

    public void DestroyCurrentObject()
    {
        if(current_object)
        {
            Destroy(current_object);
        }
        title_input.text = "";
        info_input.text = "";
    }
}
