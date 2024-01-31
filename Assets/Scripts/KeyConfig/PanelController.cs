using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public GameObject RebindKeyPanel; // Assign your RebindKeyPanel in the inspector

    void Start()
    {
        RebindKeyPanel.SetActive(false); // Disable the panel at the start of the game
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Check if the Escape key is pressed
        {
            RebindKeyPanel.SetActive(!RebindKeyPanel.activeSelf); // Toggle the panel's active state
            Debug.Log("Escキーが押されました。パネルの状態: " + (RebindKeyPanel.activeSelf ? "有効" : "無効")); // Log the state of the panel
        }
    }
}
