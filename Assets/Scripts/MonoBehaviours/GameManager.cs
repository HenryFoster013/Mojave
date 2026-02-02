using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SoundUtils;
using RISK_Utils;

public class PlayerController : MonoBehaviour
{    
    [Header("Primary References")]
    [SerializeField] MapManager _MapManager;
    [SerializeField] SoundEffectLookup SFX_Lookup;
    [SerializeField] PlayerFactionSO[] AllFactions;
    
    [Header("Main")]
    public game_state current_state;

    Dictionary<string, TerritoryInstance> territory_map = new Dictionary<string, TerritoryInstance>();
    Dictionary<string, Faction> faction_map = new Dictionary<string, Faction>();

    public enum game_state{
        IDLE,
        LOBBY,
        CLAIMANTS,
        PRIMARY,
        POSTGAME
    }
    
    void Start(){
        _MapManager.Create();
    }

    // Of format "OBJECT:COMMAND:DATA,DATA,DATA"
    // For instance "LEGION:CLAIM:THE STRIP"

    public void ProcessCommand(string command){

        command = command.ToUpper()

        string[] aspects = command.Split(':');
        string[] data = aspects[2].Split[','];

        switch(aspects[1]){
            case "CLAIM":
                Claim(GetTerritoryInstance(data[0]), GetFaction(aspects[0]));
                break;
            case "PAINT":
                Paint(GetTerritoryInstance(data[0]), GetFaction(aspects[0]));
                break;
        }
    }

    // Command Stringification

    void SetFactionDictionary(){
        foreach(PlayerFactionSO faction in AllFactions)
            faction_map.Add(faction.Name, faction);
    }

    public void SetTerritoryDictionary(Dictionary<string, TerritoryInstance> input_map){ // Computed externally by the map manager
        territory_map = input_map;
    }

    // Note that it expects all to be formatted in upper case

    public TerritoryInstance GetTerritoryInstance(string id){
        if(territory_map.TryGetValue(id, out TerritoryInstance territory))
            return territory;
        return null;
    }

    public PlayerFactionSO GetFaction(string id){
        if(faction.TryGetValue(id, out PlayerFactionSO faction))
            return faction;
        return null;    
    }

    // Command Outputs

    void Claim(TerritoryInstance territory, PlayerFactionSO faction){
        if(territory.owner == null)
            Paint(territory, faction);
    }

    void Paint(TerritoryInstance territory, PlayerFactionSO faction){
        territory.SetOwner(faction);
        territory.SetTroops(0);
        _MapManager.CheckDirtyInstances();
    }
}
