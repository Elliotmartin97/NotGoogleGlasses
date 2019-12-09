using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Contains UI elements for a tracked Object
/// </summary>
public class HUDObject : MonoBehaviour
{
    public Text object_name;

    public void SetName(string name)
    {
        object_name.text = name;
    }
}
