using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GuesserScript : MonoBehaviour
{
    private string current_guess = "";
    private string current_hint = "";
    private bool hint_used = false;
    public string answer = "TV";
    public Text input;
    public List<string> answered_titles;
    public Vector2 position;
    public GameObject score_obj;
    public bool editor_check = false;

    public void Awake()
    {
        answered_titles = new List<string>();
    }

    public void Active()
    {
        current_guess = "";
        current_hint = "";
        UpdateInput();
    }

    private void Update()
    {
        if(editor_check)
        {
            CorrectAnswer();
            editor_check = false;

        }
    }

    public void CheckAnswer()
    {
        if(current_guess.Equals(answer,StringComparison.InvariantCultureIgnoreCase))
        {
            input.color = Color.cyan;
            answered_titles.Add(answer);
            Invoke("CorrectAnswer", 1.0f);
        }
    }

    public bool CheckStringWithAnsweredTitles(string value)
    {
        for(int i = 0; i < answered_titles.Count; i++)
        {
            if(value == answered_titles[i])
            {
                return true;
            }
        }
        return false;
    }

    public void CorrectAnswer()
    {
        current_guess = "";
        GameObject score_text = Instantiate(score_obj, position, transform.rotation) as GameObject;
        score_text.transform.SetParent(GameObject.Find("Canvas").transform, false);
        score_text.transform.SetAsLastSibling();
        LerpToUI lerp = score_text.GetComponent<LerpToUI>();
        lerp.score_value = answer.Length;
        lerp.SetTarget(GameObject.Find("ScoreUI").transform.position);
        score_text.GetComponent<Text>().text = "+" + answer.Length;
        gameObject.SetActive(false);
    }

    public void AddInput(string value)
    {
        if(current_guess.Length < answer.Length)
        {
            current_guess += value;
            UpdateInput();
        }
    }

    public void TakeInput()
    {
        if(hint_used)
        {
            if(current_guess.Length - 1 > 0)
            {
                current_guess = current_guess.Substring(0, current_guess.Length - 1);
            }
        }
        else
        {
            current_guess = current_guess.Substring(0, current_guess.Length - 1);
        }
        UpdateInput();
    }

    private void UpdateInput()
    {
        string guess = "";
        for(int i = 0; i < answer.Length; i++)
        {
            if(i >= current_guess.Length)
            {
                guess += " <color=black>_</color> ";
            }
            else
            {
                if (answer[i].ToString().Equals(current_guess[i].ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    guess += " <color=green>" + current_guess[i] + "</color>";
                }
                else
                {
                    guess += " <color=red>" + current_guess[i] + "</color>";
                }
                
            }
        }
        input.text = guess;
        CheckAnswer();
    }

    private void AddPoints()
    {

    }

    public void UseHint()
    {
        hint_used = true;
        current_guess = "";
        current_guess += answer[0].ToString();
        UpdateInput();
    }
}
