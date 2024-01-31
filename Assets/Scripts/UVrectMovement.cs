using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UVrectMovement : MonoBehaviour
{
    [SerializeField] public RawImage rawImage;
    [SerializeField] private Vector2 move = new Vector2(0, 1);

    // Start is called before the first frame update
    void Start()
    {
        rawImage = GetComponent<RawImage>();    
    }

    // Update is called once per frame
    void Update()
    {
        rawImage.uvRect = new Rect(rawImage.uvRect.position + move * Time.deltaTime, rawImage.uvRect.size);
    }
}
