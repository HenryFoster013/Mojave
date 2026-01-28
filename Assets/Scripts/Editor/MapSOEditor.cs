using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapSO))]
public class MapSOEditor : Editor{

    /*public override void OnInspectorGUI(){
        DrawDefaultInspector();
        MapSO dataAsset = (MapSO)target;
        GUILayout.Space(10);

        if (GUILayout.Button("Create Texture2D Array", GUILayout.Height(25))){
            dataAsset.CreateTextureArray();
            EditorUtility.SetDirty(dataAsset);
            AssetDatabase.SaveAssets();
        }
    }*/

    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        MapSO dataAsset = (MapSO)target;
        GUILayout.Space(10);

        if (GUILayout.Button("Regenerate Additionals", GUILayout.Height(25))){
            dataAsset.RegenerateAdditionals();
            EditorUtility.SetDirty(dataAsset);
            AssetDatabase.SaveAssets();
        }
    }
}