using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableCreator : MonoBehaviour
{
    public GameObject object_score_bar;
    public List<GameObject> collected_list;
    public GuesserScript guesser;
    public int total_score = 0;
    public Text total_text;


    
    public void ShowCollected()
    {
        total_score = 0;
        if(collected_list.Count > 0)
        {
            for(int i = 0; i < collected_list.Count; i++)
            {
                Destroy(collected_list[i]);
            }
            collected_list = new List<GameObject>();
        }

        for(int i = 0; i < guesser.answered_titles.Count; i++)
        {
            GameObject new_collected = Instantiate(object_score_bar) as GameObject;
            new_collected.transform.SetParent(transform.GetChild(0), false);

            new_collected.transform.GetChild(0).GetComponent<Text>().text = guesser.answered_titles[i];
            new_collected.transform.GetChild(1).GetComponent<Text>().text = guesser.answered_titles[i].Length.ToString("F0");
            total_score += guesser.answered_titles[i].Length;

            collected_list.Add(new_collected);
        }

        total_text.text = "Total Score: <color=green>" + total_score + "</color>";
    }
}
