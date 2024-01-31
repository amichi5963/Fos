using UnityEngine;

public class ActivateLight : MonoBehaviour
{
    // �q�I�u�W�F�N�g��Light���擾
    private GameObject lightSpawnObject;

    void Start()
    {
        // Light�I�u�W�F�N�g���擾
        lightSpawnObject = transform.Find("LightSpawn").gameObject;
        // ������Ԃł�Light�I�u�W�F�N�g���A�N�e�B�u�ɐݒ�
        lightSpawnObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // �G�ꂽ�I�u�W�F�N�g�̃^�O��Player�Ȃ��
        if (other.gameObject.CompareTag("Player"))
        {
            // Light�I�u�W�F�N�g���A�N�e�B�u�ɐݒ�
            lightSpawnObject.SetActive(true);
        }
    }
}
