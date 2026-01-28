using UnityEngine;
using UnityEditor;
using System.IO;

public class TerritoryCreator{

    [MenuItem("Assets/Create/Territories from Selected Textures", false, 100)]
    public static void CreateTerritoriesFromTextures(){

        Object[] selectedObjects = Selection.objects;
        int count = 0;

        foreach (Object obj in selectedObjects){

            if (obj is Texture2D selectedTexture){

                TerritorySO newSO = ScriptableObject.CreateInstance<TerritorySO>();
                newSO.Name = selectedTexture.name;
                newSO.Source = selectedTexture;

                string path = AssetDatabase.GetAssetPath(selectedTexture);
                string directory = Path.GetDirectoryName(path);
                string newPath = Path.Combine(directory, $"{selectedTexture.name}.asset");

                newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);
                AssetDatabase.CreateAsset(newSO, newPath);
                count++;

                newSO.GenerateValues();
            }
        }

        if (count > 0){
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Assets/Create/Territory from Selected Textures", true)]
    private static bool CreateTerritoriesValidation(){
        if (Selection.objects == null || Selection.objects.Length == 0) return false;
        foreach (Object obj in Selection.objects){
            if (obj is Texture2D) return true;
        }
        return false;
    }
}