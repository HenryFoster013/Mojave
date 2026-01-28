using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Region", menuName = "World Map/Region")]
public class RegionSO : ScriptableObject
{
    public TerritorySO[] Territories;
    public Color Colour = new Color(1f,1f,1f,1f);

    [ContextMenu("Execute My Function")]
    public void SetTerritoryOwnership(){
        foreach(TerritorySO terri in Territories){
            terri.Region = this;
            EditorUtility.SetDirty(terri);
            AssetDatabase.SaveAssets();
        }
    }
}
