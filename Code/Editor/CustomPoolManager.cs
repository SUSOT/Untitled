using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PoolManager))]
public class CustomPoolManager : Editor
{
    private readonly string _listSOName = "PoolList.asset";
    private readonly string _prefabPath = "Assets/05.SO/Pool"; // 주소는 폴더에 맞게 바꾸기

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate pooling list SO"))
        {
            PoolListSO so = CreateAssetDatabase();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            PoolManager manager = target as PoolManager;
            manager.poolList = so;
        }
    }

    private PoolListSO CreateAssetDatabase()
    {
        List<PoolItemSO> loadedList = new List<PoolItemSO>();
        
        string[] assetGuids = AssetDatabase.FindAssets("", new[] {_prefabPath});

        foreach (string guid in assetGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PoolItemSO item = AssetDatabase.LoadAssetAtPath<PoolItemSO>(path);
            if (item != null)
            {
                loadedList.Add(item);
            }
        }

        PoolListSO poolList;
        poolList = AssetDatabase.LoadAssetAtPath<PoolListSO>($"{_prefabPath}/{_listSOName}");

        if (poolList == null)
        {
            poolList = ScriptableObject.CreateInstance<PoolListSO>();
            AssetDatabase.CreateAsset(poolList, $"{_prefabPath}/{_listSOName}");
            Debug.Log("Created pool list SO");
        }
        
        poolList.list = loadedList;
        EditorUtility.SetDirty(poolList); // 변경되었으니 다시 저장해줘
        return poolList;
    }
}
