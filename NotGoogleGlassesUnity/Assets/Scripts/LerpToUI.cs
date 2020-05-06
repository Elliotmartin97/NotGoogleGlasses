using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LerpToUI : MonoBehaviour
{
    public float lerp_time = 1.0f;
    private float current_lerp_time = 1.0f;
    private Vector2 start_pos;
    private Vector2 end_pos;
    public int score_value;
    public GameObject particle;

    public bool reset = false;

    public void SetTarget(Vector2 target_position)
    {
        end_pos = target_position;
    }

    

    void Start()
    {
        start_pos = transform.position;
        ResetTarget();
    }

    private void ResetTarget()
    {
        transform.position = start_pos;
        current_lerp_time = 0.0f;
    }

    void Update()
    {
        if(reset)
        {
            ResetTarget();
            reset = false;
        }

        current_lerp_time += 1 * Time.deltaTime;
        if(current_lerp_time < lerp_time)
        {
            LerpToTarget();
        }
        else
        {
            current_lerp_time = lerp_time;
            GameObject particle_obj = Instantiate(particle) as GameObject;
            particle_obj.transform.SetParent(transform.parent, false);
            particle_obj.transform.SetAsLastSibling();
            Destroy(gameObject);
        }
    }

    private void LerpToTarget()
    {
        float perc = current_lerp_time / lerp_time;
        perc = perc * perc;
        transform.position = Vector2.Lerp(start_pos, end_pos, perc);
    }
}
