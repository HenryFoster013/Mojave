using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RegionSO))]
public class RegionSOEditor : Editor{

    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        RegionSO dataAsset = (RegionSO)target;
        GUILayout.Space(10);

        if (GUILayout.Button("Set Ownership", GUILayout.Height(25))){
            dataAsset.SetTerritoryOwnership();
            EditorUtility.SetDirty(dataAsset);
            AssetDatabase.SaveAssets();
        }
    }
}