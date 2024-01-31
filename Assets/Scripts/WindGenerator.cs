using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindGenerater : MonoBehaviour
{
    private GameObject player; // �v���C���[��Prefab
    public float moveSpeed = 1.0f; // �ړ����x
    private PlayerController playerController; // PlayerController�X�N���v�g
    private Rigidbody playerRigidbody; // �v���C���[��Rigidbody
    private bool playerTouched = false; // �v���C���[���G�ꂽ���ǂ���

    void Start()
    {
        // �v���C���[��Prefab����PlayerController�X�N���v�g��Rigidbody���擾
       player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (playerTouched && playerController.getGrassesCount() > 0)
        {
            // �v���[���[�����̑��x�ŃA�^�b�`�����I�u�W�F�N�g���琂���Ɉړ�������
            Vector3 moveDirection = transform.up;
            playerRigidbody.AddForce(moveDirection * moveSpeed*Time.deltaTime*120f, ForceMode.Acceleration);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerTouched = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerTouched = false;
        }
    }
}
