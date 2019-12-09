using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the HUD objects that follow tracked objects
/// </summary>

public class HudTracker : MonoBehaviour
{
    public List<GameObject> existing_trackers;
    public GameObject tracker_parent;
    // Start is called before the first frame update
    void Start()
    {
        tracker_parent = GameObject.Find("TrackerParent").gameObject;
    }

    public void EnableTracker()
    {
        //test if theres already a tracker for type, if not then create new tracker
        GameObject new_tracker = Instantiate(Resources.Load("Prefabs/DetectorCircle")) as GameObject;
        new_tracker.transform.parent = tracker_parent.transform;
        existing_trackers.Add(new_tracker);

        //add a type parameter to pass in to set name
        new_tracker.GetComponent<HUDObject>().SetName("TestName");
    }

    public void UpdateTracker(int index, Vector3 position)
    {
        //move tracker to be over the objects postion
    }

    public void DisableTracker(int index)
    {
        existing_trackers[index].SetActive(false);
    }
}
