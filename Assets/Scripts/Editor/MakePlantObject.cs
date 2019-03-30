using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakePlantObject
{
    [MenuItem("Assets/Create/Plant Object")]
    public static void Create()
    {
        PlantObject asset = ScriptableObject.CreateInstance<PlantObject>();
        AssetDatabase.CreateAsset(asset, "Assets/NewPlantObject.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

}