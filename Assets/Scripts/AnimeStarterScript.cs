using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimeStarterScript : MonoBehaviour
{
    [SerializeField] Animation anim;
    [SerializeField] float Deley = 0f;
    [SerializeField] GameObject[] animeSounds;
    bool done = false;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = Deley;
        foreach (AnimationState state in anim)
        {
            state.speed = 0.0F;
        }
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;    

        if (timer <= 0 &&!done)
        {
            done = true;
            foreach (AnimationState state in anim)
            {
                state.speed = 1.0F;
            }

            if (animeSounds.Length > 0)
                foreach (GameObject gameObject in animeSounds)
                {
                    gameObject.SetActive(!gameObject.activeSelf);
                }
        }
    }
}