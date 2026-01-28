using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerritorySO))]
public class TerritorySOEditor : Editor{

    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        TerritorySO dataAsset = (TerritorySO)target;
        GUILayout.Space(10);

        if (GUILayout.Button("Regenerate", GUILayout.Height(25))){
            dataAsset.GenerateValues();
            EditorUtility.SetDirty(dataAsset);
            AssetDatabase.SaveAssets();
        }
    }
}