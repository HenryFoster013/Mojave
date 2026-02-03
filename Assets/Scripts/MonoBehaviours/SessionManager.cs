using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SoundUtils;
using RISK_Utils;

public class SessionManager : MonoBehaviour
{    
    [Header("Primary References")]
    [SerializeField] TerminalController _TerminalController;
    [SerializeField] MapManager _MapManager;
    [SerializeField] SoundEffectLookupSO SFX_Lookup;
    [SerializeField] PlayerFactionSO[] AllFactions;
    
    [Header("Main")]
    public game_state current_state;

    Dictionary<string, TerritoryInstance> territory_map = new Dictionary<string, TerritoryInstance>();
    Dictionary<string, PlayerFactionSO> faction_map = new Dictionary<string, PlayerFactionSO>();

    const string err_header = "<color=red>ERR: ";
    const string err_footer = "</color>";

    public bool admin {get; private set;}

    public enum game_state{
        IDLE,
        LOBBY,
        CLAIMANTS,
        PRIMARY,
        POSTGAME
    }
    
    void Start(){
        admin = true;
        SetFactionDictionary();
        _MapManager.Create();
    }

    // ----- // --- COMMANDS --- // ----- //

    // Of format "OBJECT:COMMAND:DATA,DATA,DATA"
    // For instance "LEGION:CLAIM:THE STRIP"

    public void ProcessCommand(string command){

        command = command.ToUpper();
        
        string to_log = err_header + "Command not recognised." + err_footer;;
        string[] aspects = command.Split('.');

        if(aspects.Length < 3){
            _TerminalController.LogLine(err_header + "Invalid format. Commands must include .." + err_footer);
            return;
        }
        
        string[] data = aspects[2].Split(':');

        switch(aspects[1]){
            case "ADMIN":
                to_log = SetAdmin(data);
                break;
            case "CLAIM":
                to_log = Claim(GetTerritoryInstance(data[0]), GetFaction(aspects[0]));
                break;
            case "PAINT":
                to_log = Paint(GetTerritoryInstance(data[0]), GetFaction(aspects[0]));
                break;
        }

        _TerminalController.LogLine(to_log);
    }

    // Command Stringification

    void SetFactionDictionary(){
        foreach(PlayerFactionSO faction in AllFactions)
            faction_map.Add(faction.ID, faction);
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
        if(faction_map.TryGetValue(id, out PlayerFactionSO faction))
            return faction;
        return null;    
    }

    // Common Command Validation

    string RequireAdmin(){
        if(!admin)
            return err_header + "Requires admin." + err_footer;
        return "";
    }
    
    string TerritoryFactionValidation(TerritoryInstance territory, PlayerFactionSO faction){
        if(territory == null)
            return err_header + "Invalid territory." + err_footer;
        if(faction == null)
            return err_header + "Invalid faction." + err_footer;
        return "";
    }

    // COMMAND ACTION //

    // Management

    string SetAdmin(string[] data){
        if(data.Length == 0)
            admin = !admin;
        else{
            if(data[0] == "TRUE")
                admin = true;
            else if(data[0] == "FALSE")
                admin = false;
            else
                return err_header + "Invalid argument." + err_footer;
        }

        if(admin)
            return "Admin mode activated";
        else
            return "Admin mode deactivated";
    }

    // Territory Ownership

    string Claim(TerritoryInstance territory, PlayerFactionSO faction){

        string validation = TerritoryFactionValidation(territory, faction);
        if(validation != "")
            return validation;

        if(territory.owner == null){
            ChangeOwnership(territory, faction);
            return faction.Name + " claimed " + territory.Name();
        }

        return err_header + "Territory already claimed." + err_footer;
    }

    string Paint(TerritoryInstance territory, PlayerFactionSO faction){

        string validation = RequireAdmin();
        if(validation != "")
            return validation;
        
        validation = TerritoryFactionValidation(territory, faction);
        if(validation != "")
            return validation;

        territory.SetTroops(0);
        ChangeOwnership(territory, faction);

        return faction.Name + " painted " + territory.Name();
    }

    void ChangeOwnership(TerritoryInstance territory, PlayerFactionSO faction){
        territory.SetOwner(faction);
        _MapManager.CheckDirtyInstances();
    }
}