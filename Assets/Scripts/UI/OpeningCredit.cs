using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class OpeningCredit : MonoBehaviour
{
    [SerializeField] CinemachineDollyCart cm_cart;

    [SerializeField] float Start_Pos;
    [SerializeField] float CircleLogo_Pos;
    [SerializeField] float Final_Pos;

    [SerializeField] int sceneNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Start_Pos = Start_Pos_Obj.transform.position;
        //CircleLogo_Pos = CircleLogo_Pos_Obj.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            sceneNum++;

            if (sceneNum == 1) DOTween.To(() => cm_cart.m_Position, x => cm_cart.m_Position = x, CircleLogo_Pos, 1f).SetEase(Ease.InOutQuart);
            if (sceneNum == 2) DOTween.To(() => cm_cart.m_Position, x => cm_cart.m_Position = x, Final_Pos, 1f).SetEase(Ease.InOutQuart);
        }

    }
}
