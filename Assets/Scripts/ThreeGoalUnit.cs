using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeGoalUnit : MonoBehaviour
{
    [SerializeField] GameObject[] goal;
    [SerializeField] float ringTime;
    [SerializeField] GoalUnit[] rings;
    float[] ringTimers;
    // Start is called before the first frame update
    void Start()
    {
        ringTimers = new float[rings.Length];
        for (int i = 0; i < ringTimers.Length; i++)
        {
            ringTimers[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ringTimers.Length; i++)
        {
            ringTimers[i] -= Time.deltaTime;
        }
        for (int i = 0; i < rings.Length; i++)
        {
            if (rings[i].IsRinging()) { ringTimers[i] = ringTime; }
        }
        bool isGoaling = true;
        for (int i = 0; i < ringTimers.Length; i++)
        {
            if (ringTimers[i] < 0f) isGoaling = false;
        }
        if (isGoaling)
        {
            //Debug.Log("Clear");
            if(goal.Length > 0)
            foreach (GameObject gameObject in goal)
            {
                gameObject.SetActive(!gameObject.activeSelf);
            }
            this.gameObject.SetActive(false);
        }
    }
}