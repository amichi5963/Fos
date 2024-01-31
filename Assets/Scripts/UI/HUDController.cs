using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HUDController : MonoBehaviour
{
    PlayerController playerController;

    [SerializeField] GameObject KinokoManual;
    [SerializeField] GameObject GrassManual;
    [SerializeField] GameObject StoneManual;
    [SerializeField] GameObject GrassAtack;
    [SerializeField] GameObject StoneAtack;

    private void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    private void Update()
    {
        GrassAtack.SetActive(false);
        StoneAtack.SetActive(false);
        GrassManual.SetActive(false);
        StoneManual.SetActive(false);
        KinokoManual.SetActive(true);

        if (playerController.getStonesCount() > 0)
        {
            KinokoManual.SetActive(false);
            StoneManual.SetActive(true);
            if(playerController.getStonesCount()>1)
            {
                StoneAtack.SetActive(true);
            }
        }

        if(playerController.getGrassesCount() > 0)
        {
            KinokoManual.SetActive(false);
            GrassManual.SetActive(true);
            if(playerController.getGrassesCount()>1)
            {
                GrassAtack.SetActive(true);
            }
        }
    }
}