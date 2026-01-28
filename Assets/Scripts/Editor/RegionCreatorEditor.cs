using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class RegionCreatorEditor : Editor{

    [MenuItem("Assets/Create Regions from Selected Folders", false, 10)]
    public static void CreateRegionsFromFolders(){

        Object[] selectedObjects = Selection.GetFiltered<Object>(SelectionMode.Assets);        
        int createdCount = 0;

        foreach (Object obj in selectedObjects){

            string path = AssetDatabase.GetAssetPath(obj);
            
            if (!AssetDatabase.IsValidFolder(path)) continue;

            string folderName = Path.GetFileName(path);
            string assetPath = Path.Combine(path, folderName + ".asset");

            string[] guids = AssetDatabase.FindAssets("t:TerritorySO", new[] { path });
            List<TerritorySO> territoriesFound = new List<TerritorySO>();

            foreach (string guid in guids){

                string tPath = AssetDatabase.GUIDToAssetPath(guid);
                TerritorySO territory = AssetDatabase.LoadAssetAtPath<TerritorySO>(tPath);
                if (territory != null)
                    territoriesFound.Add(territory);

            }

            RegionSO newRegion = ScriptableObject.CreateInstance<RegionSO>();
            newRegion.Territories = territoriesFound.ToArray();
            AssetDatabase.CreateAsset(newRegion, assetPath);
            newRegion.SetTerritoryOwnership();

            createdCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Successfully created {createdCount} RegionSO(s).");
    }

    [MenuItem("Assets/Create Regions from Selected Folders", true)]
    public static bool ValidateCreateRegionsFromFolders()
    {
        return Selection.activeObject != null && AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject));
    }
}