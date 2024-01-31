using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CounterScript : MonoBehaviour
{
    TextMeshProUGUI proText; 
    [SerializeField] int MAX_Light;

    int lightCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        proText = GetComponent<TextMeshProUGUI>();
        proText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        lightCount = PlayerPrefs.GetInt("InStage_HoldLight", 0);
        string str = lightCount.ToString()+"/"+MAX_Light.ToString();
        proText.text = str;
    }
}
