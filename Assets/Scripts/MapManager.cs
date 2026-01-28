using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RISK_Utils;

public class MapManager : MonoBehaviour
{   
    [Header("Main")]
    [SerializeField] MapSO MapData;

    [Header("Rendering")]
    [SerializeField] Material DisplayMaterial;
    [SerializeField] Material ComputeMaterial;
    [SerializeField] GameObject NametagPrefab;
    [SerializeField] Transform NametagHook;

    RenderTexture rendered_territories;
    List<TerritoryInstance> territory_instances = new List<TerritoryInstance>();
    
    // SETUP //

    void Start(){
        CreateRenderedTexture();
        SetupMap();
    }

    void CreateRenderedTexture(){
        rendered_territories = new RenderTexture(MapData.Coloured.width, MapData.Coloured.height, 0);
        rendered_territories.filterMode = FilterMode.Point;
        rendered_territories.Create();
        Graphics.Blit(Texture2D.whiteTexture, rendered_territories);
        DisplayMaterial.SetTexture("_TileColours", rendered_territories);
    }

    public void SetupMap(){
        territory_instances = MapData.GenerateInstances();
        CreateNameTags();
        CheckDirtyInstances();
    }

    void CreateNameTags(){
        foreach(TerritoryInstance territory in territory_instances){
            GameObject tag = GameObject.Instantiate(NametagPrefab);
            tag.SetActive(true);
            tag.transform.SetParent(NametagHook);
            tag.name = territory.definition.Name;
            tag.transform.localPosition = new Vector3(territory.definition.MedianX, territory.definition.MedianY, 0f);
            tag.GetComponent<NametagController>().SetName(territory.definition.Name);
        }
    }

    // UPDATING //
    
    void CheckDirtyInstances(){
        foreach(TerritoryInstance territory in territory_instances){
            if(territory.dirty)
                UpdateTerritory(territory, territory.definition.Region.Colour);
        }
    }

    public void UpdateTerritory(TerritoryInstance territory, Color new_colour){

        RenderTexture tempRT = RenderTexture.GetTemporary(rendered_territories.width, rendered_territories.height);

        ComputeMaterial.SetTexture("_ColourMap", MapData.Coloured);
        ComputeMaterial.SetVector("_TargetID", new Vector4(territory.definition.Colour.r/255f, territory.definition.Colour.g/255f, territory.definition.Colour.b/255f, 1f));
        ComputeMaterial.SetColor("_NewColour", new_colour);

        Graphics.Blit(rendered_territories, tempRT, ComputeMaterial);
        Graphics.Blit(tempRT, rendered_territories);
        RenderTexture.ReleaseTemporary(tempRT);

        territory.Clean();
    }
}
