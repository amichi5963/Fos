using UnityEngine;

public class LightGenerator : MonoBehaviour
{
    public GameObject lightPrefab; // LightPrefab���C���X�y�N�^�[����A�^�b�`���Ă�������

    private void Start()
    {
        // ���C�g�𐶐�
        GameObject light = Instantiate(lightPrefab, transform.position, Quaternion.identity);
    }
}
