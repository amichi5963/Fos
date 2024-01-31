using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindSaveManager : MonoBehaviour
{
    // �ΏۂƂȂ�InputActionAsset
    [SerializeField] private InputActionAsset _actionAsset;

    // �㏑�����̕ۑ���
    [SerializeField] private string _savePath = "InputActionOverrides.json";

    // �㏑�����̕ۑ�
    public void Save()
    {
        if (_actionAsset == null) return;

        // InputActionAsset�̏㏑�����̕ۑ�
        var json = _actionAsset.SaveBindingOverridesAsJson();

        // �t�@�C���ɕۑ�
        var path = Path.Combine(Application.persistentDataPath, _savePath);
        File.WriteAllText(path, json);
    }

    // �㏑�����̓ǂݍ���
    public void Load()
    {
        if (_actionAsset == null) return;

        // �t�@�C������ǂݍ���
        var path = Path.Combine(Application.persistentDataPath, _savePath);
        if (!File.Exists(path)) return;

        var json = File.ReadAllText(path);

        // InputActionAsset�̏㏑������ݒ�
        _actionAsset.LoadBindingOverridesFromJson(json);
    }
}