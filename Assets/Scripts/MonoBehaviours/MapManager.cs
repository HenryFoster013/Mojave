using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;
using RISK_Utils;

public class MapManager : MonoBehaviour
{   
    [Header("Main")]
    [SerializeField] MapSO MapData;
    [SerializeField] SoundEffectLookupSO SoundEffectLookup;

    [Header("Rendering")]
    [SerializeField] Material DisplayMaterial;
    [SerializeField] Material ComputeMaterial;

    [Header("Nametags")]
    [SerializeField] GameObject NametagPrefab;
    [SerializeField] Transform NametagHook;

    RenderTexture rendered_territories, rendered_regions;
    List<TerritoryInstance> territory_instances = new List<TerritoryInstance>();
    List<NametagController> nametags = new List<NametagController>();

    bool render_mode; // false == Players, true == Regions
    
    // SETUP //

    void Start(){
        SetupRenders();
        SetupMap();
    }

    void SetupRenders(){
        CreateRenderedTexture(ref rendered_territories);
        CreateRenderedTexture(ref rendered_regions);
        DisplayMaterial.SetTexture("_TileColours", rendered_territories);
    }

    void CreateRenderedTexture(ref RenderTexture render_to){
        render_to = new RenderTexture(MapData.Coloured.width, MapData.Coloured.height, 0);
        render_to.filterMode = FilterMode.Point;
        render_to.Create();
        Graphics.Blit(Texture2D.whiteTexture, render_to);
    }

    public void SetupMap(){
        render_mode = false;
        territory_instances = MapData.GenerateInstances();
        CreateRegionMap();
        CreateNameTags();
        CheckDirtyInstances();
    }

    void CreateRegionMap(){
        foreach(TerritoryInstance territory in territory_instances)
            UpdateTerritory(territory, true);
    }

    void CreateNameTags(){
        foreach(TerritoryInstance territory in territory_instances){
            GameObject tag = GameObject.Instantiate(NametagPrefab);
            tag.transform.SetParent(NametagHook);
            tag.transform.localPosition = new Vector3(territory.definition.MedianX, territory.definition.MedianY, 0f);

            NametagController tag_controller = tag.GetComponent<NametagController>();
            tag_controller.SetName(territory.Name());
            tag_controller.UpdateMode(render_mode);
            nametags.Add(tag_controller);
        }
    }

    void MarkAllDirty(){
        foreach(TerritoryInstance territory in territory_instances)
            territory.Dirty();
    }

    // UPDATING //
    
    void CheckDirtyInstances(){
        foreach(TerritoryInstance territory in territory_instances){
            if(territory.dirty){
                UpdateTerritory(territory, false);   
            }
        }
    }

    public void UpdateTerritory(TerritoryInstance territory, bool region_mode){

        RenderTexture render_to = rendered_territories;
        Color set_color = territory.FactionColour();
        if(region_mode){
            set_color = territory.RegionColour();
            render_to = rendered_regions;
        }

        RenderTexture tempRT = RenderTexture.GetTemporary(render_to.width, render_to.height);

        ComputeMaterial.SetTexture("_ColourMap", MapData.Coloured);
        ComputeMaterial.SetVector("_TargetID", new Vector4(territory.definition.Colour.r/255f, territory.definition.Colour.g/255f, territory.definition.Colour.b/255f, 1f));
        ComputeMaterial.SetColor("_NewColour", set_color);

        Graphics.Blit(render_to, tempRT, ComputeMaterial);
        Graphics.Blit(tempRT, render_to);
        RenderTexture.ReleaseTemporary(tempRT);

        if(!region_mode)
            territory.Clean();
    }

    // INTERACTION //

    public void FlipRenderMode(){SetRenderMode(!render_mode);}

    public void SetRenderMode(bool new_mode){

        if(render_mode == new_mode)
            return;
        
        render_mode = new_mode;
        foreach(NametagController tag in nametags)
            tag.UpdateMode(render_mode);

        if(render_mode){
            PlaySFX("pipboy_light_on", SoundEffectLookup);
            DisplayMaterial.SetTexture("_TileColours", rendered_regions);
        }
        else{
            PlaySFX("pipboy_light_off", SoundEffectLookup);
            DisplayMaterial.SetTexture("_TileColours", rendered_territories);
        }
    }

    public void UpdateNametagRotation(float new_rot){
        foreach(NametagController tag in nametags)
            tag.UpdateRotation(new_rot);
    }
}
