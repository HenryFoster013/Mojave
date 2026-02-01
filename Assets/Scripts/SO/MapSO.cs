using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RISK_Utils;
using UnityEditor;

[CreateAssetMenu(fileName = "New Map", menuName = "World Map/Map")]
public class MapSO : ScriptableObject
{
    [Header("Main")]
    public RegionSO[] Regions;
    
    [Header("Textures")]
    public Texture2D Base;
    public Texture2D Overlay;
    public Texture2D Coloured;

    public List<TerritoryInstance> GenerateInstances(){
        List<TerritoryInstance> territory_instances = new List<TerritoryInstance>();

        foreach(RegionSO region in Regions){
            RegionInstance region_instance = new RegionInstance(region);
            foreach(TerritorySO territory in region.Territories){
                TerritoryInstance new_terri = new TerritoryInstance(territory);
                region_instance.AddTerritory(new_terri);
                territory_instances.Add(new_terri);
            }
        }

        return territory_instances;
    }

    List<TerritorySO> FetchTerritoriesSO(){
        List<TerritorySO> territories = new List<TerritorySO>();
        foreach(RegionSO region in Regions){
            foreach(TerritorySO territory in region.Territories)
                territories.Add(territory);
        }
        return territories;
    }

    Color32 GetColour(string terri_name, ref string[] lines){

        bool found_line = false;
        Color32 result = new Color32(0,0,0,255);

        for(int i = 0; i < lines.Length && !found_line; i++){
            if(lines[i] != ""){
                string referenced_name = lines[i].Split(']')[1].Split(':')[0].Substring(1);
                found_line = (referenced_name == terri_name);
                if(found_line){
                    string[] channels = lines[i].Split('(')[1].Replace(")", "").Split(',');
                    int r = int.Parse(channels[0].Trim());
                    int g = int.Parse(channels[1].Trim());
                    int b = int.Parse(channels[2].Trim());
                    result = new Color32((byte)r, (byte)g, (byte)b, 255);
                }
            }
        }

        return result;
    }

    public void RegenerateAdditionals(){
        foreach(RegionSO region in Regions){
            foreach(TerritorySO territory in region.Territories){
                territory.GenerateValues();
            }
        }
    }
}
