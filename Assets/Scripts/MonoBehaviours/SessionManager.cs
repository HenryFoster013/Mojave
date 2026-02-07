using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SoundUtils;
using RISK_Utils;
using static GenericUtils;

public class SessionManager : MonoBehaviour
{    
    public bool DefaultAdmin = true;
    
    [Header("Primary References")]
    [SerializeField] TerminalController _TerminalController;
    [SerializeField] MapManager _MapManager;
    [SerializeField] PlayerController _PlayerController;
    [SerializeField] SoundEffectLookupSO SFX_Lookup;

    [Header("Secondary References")]
    [SerializeField] LobbyController _LobbyController;

    [Header("Factions")]
    public bool UseBonusFactions = false;
    [SerializeField] List<PlayerFactionSO> StandardFactions = new List<PlayerFactionSO>();
    [SerializeField] List<PlayerFactionSO> BonusFactions = new List<PlayerFactionSO>();
    
    [Header("Main")]
    public game_state current_state;

    Dictionary<string, TerritoryInstance> territory_map = new Dictionary<string, TerritoryInstance>();
    Dictionary<string, PlayerFactionSO> faction_map = new Dictionary<string, PlayerFactionSO>();

    public bool admin {get; private set;}

    public enum game_state{
        NULL,
        IDLE,
        LOBBY,
        CLAIMANTS,
        PRIMARY,
        POSTGAME
    }

    // ----- // INITIAL // ----- //

    void Start(){
        admin = DefaultAdmin;
        SetFactionDictionary();
        _MapManager.Create();
        LoadGameState();
    }

    void LoadGameState(){
        switch(current_state){
            case game_state.LOBBY:
                LoadLobby();
                break;
            case game_state.CLAIMANTS:
                LoadClaimants();
                break;
        }
    }

    void LoadLobby(){
        _MapManager.SetRenderMode(true, false);
        _LobbyController.LoadFactions(GenerateFactionsList());
    }

    List<PlayerFactionSO> GenerateFactionsList(){
        List<PlayerFactionSO> factions = new List<PlayerFactionSO>();
        factions.AddRange(StandardFactions);
        if(UseBonusFactions)
            factions.AddRange(BonusFactions);
        return factions;
    }

    void LoadClaimants(){
       _MapManager.SetRenderMode(false, false);
    }

    void LoadPrimary(){ }

    void LoadPostgame(){ }

    // ----- // UPDATE // ----- //

    void Update(){
        IdleLogic();
        GameStateFunctionality();
    }

    void IdleLogic(){
        if(Input.GetKeyDown("/"))
            _TerminalController.FlipFlopEnabled(); 
        if(_TerminalController.Focused()){
            if(Input.GetKeyDown(KeyCode.UpArrow))
                _TerminalController.ScrollCommands(false);
            if(Input.GetKeyDown(KeyCode.DownArrow))
                _TerminalController.ScrollCommands(true);
        }
    }
    
    void GameStateFunctionality(){
        switch(current_state){
            case game_state.LOBBY:
                LobbyUpdate();
                break;
            case game_state.CLAIMANTS:
                ClaimantUpdate();
                break;
        }
    }

    void LobbyUpdate(){ }

    void ClaimantUpdate(){
        _PlayerController.RunLogic();
    }

    void PrimaryUpdate(){ }

    void PostgameUpdate(){ }

    // ----- // --- COMMAND MANAGEMENT --- // ----- //

    // Of format "COMMAND.VALUE.VALUE.VALUE"
    // For instance "CLAIM.THE STRIP.LEGION"
    // Commands automatically formatted to uppercase

    public void ProcessCommand(string input_command){

        input_command = input_command.ToUpper();
        
        string to_log = ErrorWrap("Command not recognised.");
        string[] commands = input_command.Split('.');

        switch(commands[0]){
            case "ADMIN":
                to_log = SetAdmin(commands);
                break;
            case "CLAIM":
                to_log = Claim(commands);
                break;
            case "PAINT":
                to_log = Paint(commands);
                break;
            case "STATE":
                to_log = ChangeState(commands);
                break;
            case "BONUS FACTIONS":
                to_log = ToggleBonusFactions(commands);
                break;
        }

        _TerminalController.LogLine(to_log);
    }
    
    void ChangeOwnership(TerritoryInstance territory, PlayerFactionSO faction){
        territory.SetOwner(faction);
        _MapManager.CheckDirtyInstances();
    }

    // Command Stringification

    void SetFactionDictionary(){
        foreach(PlayerFactionSO faction in StandardFactions)
            faction_map.Add(faction.ID, faction);
        foreach(PlayerFactionSO faction in BonusFactions)
            faction_map.Add(faction.ID, faction);
    }

    public void SetTerritoryDictionary(Dictionary<string, TerritoryInstance> input_map){ // Computed externally by the map manager
        territory_map = input_map;
    }

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

    private (string error, TerritoryInstance territory, PlayerFactionSO faction) TryGetArgs(string[] commands)
    {
        if (commands.Length < 2)
            return (ErrorWrap("Must denote a territory!"), null, null);

        var territory = GetTerritoryInstance(commands[1]);
        var faction = commands.Length > 2 ? GetFaction(commands[2]) : _PlayerController.OurFaction();

        return (null, territory, faction);
    }

    // Common Command Validation

    string RequireAdmin(){
        if(!admin)
            return ErrorWrap("Requires admin.");
        return "";
    }
    
    string TerritoryFactionValidation(TerritoryInstance territory, PlayerFactionSO faction){
        if(territory == null)
            return ErrorWrap("Invalid territory.");
        if(faction == null)
            return ErrorWrap("Invalid faction.");
        return "";
    }

    // Validation //

    bool ReadError(string validation, out string result) {
        result = validation;
        return !string.IsNullOrEmpty(validation);
    }

    void ToggleBooleanCommand(ref bool boolean_value, string[] commands){
        if(commands.Length < 2)
            boolean_value = !boolean_value;
        else{
            if(commands[1] == "TRUE")
                boolean_value = true;
            else if(commands[1] == "FALSE")
                boolean_value = false;
            else
                boolean_value = !boolean_value;
        }
    }

    // ----- // --- COMMAND FUNCTIONS --- // ----- //

    // Management //

    /*
        "bonus factions" (toggles on/off).
        "bonus factions.VALUE" - true, false
    */
    string ToggleBonusFactions(string[] commands){
        ToggleBooleanCommand(ref UseBonusFactions, commands);
        _LobbyController.LoadFactions(GenerateFactionsList());
        return $"Bonus factions set to {UseBonusFactions}";
    }

    /*
        "admin" (toggles on/off).
        "admin.VALUE" - true, false
    */
    string SetAdmin(string[] command){
        if(command.Length < 2)
            admin = !admin;
        else{
            if(command[1] == "TRUE")
                admin = true;
            else if(command[1] == "FALSE")
                admin = false;
            else
                admin = !admin;
        }

        if(admin)
            return "Admin mode activated";
        else
            return "Admin mode deactivated";
    }

    /*
        "state.VALUE" - idle, lobby, claimants, primary, postgame
    */
    string ChangeState(string[] commands){
        if (ReadError(RequireAdmin(), out var adminErr)) return adminErr;
        if(commands.Length < 2)
            return ErrorWrap("Need to specify a state");

        game_state new_state = game_state.NULL;

        switch(commands[1]){
            case "LOBBY":
                new_state = game_state.LOBBY;
                if(new_state != current_state) LoadLobby();
                break;
            case "CLAIMANTS":
                new_state = game_state.CLAIMANTS;
                if(new_state != current_state) LoadClaimants();
                break;
            case "PRIMARY":
                new_state = game_state.PRIMARY;
                if(new_state != current_state) LoadPrimary();
                break;
            case "POSTGAME":
                new_state = game_state.POSTGAME;
                if(new_state != current_state) LoadPostgame();
                break;
            case "IDLE":
                new_state = game_state.IDLE;
                break;
        }

        if(new_state == game_state.NULL)
            return ErrorWrap("Unknown state.");

        current_state = new_state;    
        return $"State set to {current_state}";
    }

    // Territory Ownership //

    /*
        "claim.territory" (local users faction claims territory)
        "claim.territory.faction" (denoted faction claims territory)
    */
    string Claim(string[] commands){
        var (error, territory, faction) = TryGetArgs(commands);
        if (error != null) return error;
        if (ReadError(TerritoryFactionValidation(territory, faction), out var validationError)) return validationError;

        if (territory.owner == null){
            ChangeOwnership(territory, faction);
            return $"{faction.Name} claimed {territory.Name()}";
        }
        return ErrorWrap("Territory already claimed.");
    }

    /*
        "paint.territory" (local users faction paints territory)
        "paint.territory.faction" (denoted faction paints territory)
    */
    string Paint(string[] commands)
    {
        var (error, territory, faction) = TryGetArgs(commands);
        if (error != null) return error;

        if (ReadError(RequireAdmin(), out var adminErr)) return adminErr;
        if (ReadError(TerritoryFactionValidation(territory, faction), out var valErr)) return valErr;

        territory.SetTroops(0);
        ChangeOwnership(territory, faction);
        return $"{faction.Name} painted {territory.Name()}";
    }
}