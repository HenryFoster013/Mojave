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
    Dictionary<Color, TerritoryInstance> colour_territories_map = new Dictionary<Color, TerritoryInstance>();
    List<NametagController> nametags = new List<NametagController>();
    List<Material> world_item_materials = new List<Material>();

    bool render_mode; // false == Players, true == Regions

    const float board_world_scale = 30f;
    Vector3 board_offset;
    
    // SETUP //

    void Start(){
        Defaults();
        SetupRenders();
        SetupMap();
    }

    void Defaults(){
        world_item_materials = new List<Material>();
        territory_instances = new List<TerritoryInstance>();
        colour_territories_map = new Dictionary<Color, TerritoryInstance>();
        nametags = new List<NametagController>();
        render_mode = false;
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
        board_offset = 0.5f * board_world_scale * new Vector3(1f,0f,1f);
        render_mode = false;
        territory_instances = MapData.GenerateInstances();
        Shader.DisableKeyword("_REGION_MODE");
        CreateRegionMap();
        CreateNameTags();
        CheckDirtyInstances();
    }

    void CreateRegionMap(){
        foreach(TerritoryInstance territory in territory_instances){
            colour_territories_map.Add(territory.definition.Colour, territory);
            UpdateTerritory(territory, true);
            if(territory.definition.WorldItem != null){
                territory.definition.WorldItem.SetColor("_Region_Colour", territory.definition.Region.Colour);
                world_item_materials.Add(territory.definition.WorldItem);
            }
        }
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
    
    public void CheckDirtyInstances(){
        foreach(TerritoryInstance territory in territory_instances){
            if(territory.dirty)
                UpdateTerritory(territory, false);   
        }
    }

    public void UpdateTerritory(TerritoryInstance territory, bool region_mode){

        RenderTexture render_to = rendered_territories;
        Color set_color = territory.FactionColour();

        if(!territory.region.complete && territory.owner != null)
            set_color.a = territory.owner.IncompleteAlpha;

        if(territory.selected)
            set_color.a -= 0.33f;

        if(region_mode){
            set_color = territory.RegionColour();
            render_to = rendered_regions;
        }

        if(territory.definition.WorldItem != null)
            territory.definition.WorldItem.SetColor("_Base_Colour", set_color);

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
            Shader.EnableKeyword("_REGION_MODE");
        }
        else{
            PlaySFX("pipboy_light_off", SoundEffectLookup);
            DisplayMaterial.SetTexture("_TileColours", rendered_territories);
            Shader.DisableKeyword("_REGION_MODE");
        }

    }

    public void UpdateNametagRotation(float new_rot){
        foreach(NametagController tag in nametags)
            tag.UpdateRotation(new_rot);
    }

    public TerritoryInstance GetTerritoryAtPoint(Vector3 point){

        Vector3 normalised_point = (point + board_offset) / board_world_scale;
        int x = (int)(normalised_point.x * MapData.Coloured.width);
        int y = (int)(normalised_point.z * MapData.Coloured.height);

        if(colour_territories_map.TryGetValue(MapData.Coloured.GetPixel(x, y), out TerritoryInstance territory))
            return territory;
        return null;
    }
}
