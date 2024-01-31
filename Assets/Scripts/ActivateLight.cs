using UnityEngine;

public class ActivateLight : MonoBehaviour
{
    // 子オブジェクトのLightを取得
    private GameObject lightSpawnObject;

    void Start()
    {
        // Lightオブジェクトを取得
        lightSpawnObject = transform.Find("LightSpawn").gameObject;
        // 初期状態ではLightオブジェクトを非アクティブに設定
        lightSpawnObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // 触れたオブジェクトのタグがPlayerならば
        if (other.gameObject.CompareTag("Player"))
        {
            // Lightオブジェクトをアクティブに設定
            lightSpawnObject.SetActive(true);
        }
    }
}
