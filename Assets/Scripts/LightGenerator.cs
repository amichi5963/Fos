using UnityEngine;

public class LightGenerator : MonoBehaviour
{
    public GameObject lightPrefab; // LightPrefabをインスペクターからアタッチしてください

    private void Start()
    {
        // ライトを生成
        GameObject light = Instantiate(lightPrefab, transform.position, Quaternion.identity);
    }
}
