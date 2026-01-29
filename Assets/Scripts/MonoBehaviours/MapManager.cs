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
    float nametag_rotation = 0f;

    bool render_mode; // false == Players, true == Continents
    
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
        NametagHook.gameObject.SetActive(render_mode);
        territory_instances = MapData.GenerateInstances();
        CreateNameTags();
        CreateRegionMap();
        CheckDirtyInstances();
    }

    void CreateRegionMap(){
        foreach(TerritoryInstance territory in territory_instances)
            UpdateTerritory(territory, true);
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
        Color set_color = territory.GetFactionColour();
        if(region_mode){
            set_color = territory.GetRegionColour();
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
        NametagHook.gameObject.SetActive(render_mode);

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
        nametag_rotation = -new_rot;
        RotateTags();
    }

    void RotateTags(){
        foreach(Transform t in NametagHook)
            t.localEulerAngles = new Vector3(0f, 0f, nametag_rotation);
    }
}
