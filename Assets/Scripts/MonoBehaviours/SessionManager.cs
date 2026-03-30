using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SoundUtils;
using RISK_Utils;
using static GenericUtils;

public class SessionManager : MonoBehaviour
{    
    public bool Offline = true;
    public int BotCount = 3;
    
    public bool DefaultAdmin = true;
    
    [Header("Primary References")]
    [SerializeField] TerminalController _TerminalController;
    [SerializeField] MapManager _MapManager;
    [SerializeField] PlayerController _PlayerController;
    [SerializeField] SoundEffectLookupSO SFX_Lookup;

    [Header("Secondary References")]
    [SerializeField] LobbyController _LobbyController;

    [Header("Factions")]
    [SerializeField] List<PlayerFactionSO> AllFactions = new List<PlayerFactionSO>();
    Queue<PlayerFactionSO> free_factions = new Queue<PlayerFactionSO>();
    
    [Header("Networked")]
    public game_state current_state;
    [SerializeField] public GameObject PlayerInstancePrefab;
    public PlayerInstance OurInstance;
    public List<PlayerInstance> OtherInstances;

    Dictionary<string, TerritoryInstance> territory_map = new Dictionary<string, TerritoryInstance>();
    Dictionary<string, PlayerFactionSO> faction_map = new Dictionary<string, PlayerFactionSO>();

    bool admin;
    int player_count;
    int auto_id = -1;

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
        SetupOurInstance();
        _MapManager.Create();
        LoadGameState();
    }

    void SetupOurInstance(){
        OurInstance = GameObject.Instantiate(PlayerInstancePrefab).GetComponent<PlayerInstance>();
        OurInstance.Username = "Local Player";
        OurInstance.SetFaction(free_factions.Dequeue());
        OurInstance.ID = NewPlayerInstanceID();
        OurInstance._SessionManager = this;
        player_count = 1;
    }

    int NewPlayerInstanceID(){
        auto_id++;
        return auto_id;
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
        _LobbyController.RegenerateUI();
    }

    // Loading States //

    void CloseAllStates(){
        _LobbyController.CloseLobby();
    }

    void LoadClaimants(){
        CloseAllStates();
       _MapManager.SetRenderMode(false, false);
    }

    void LoadPrimary(){ }

    void LoadPostgame(){ }

    // ----- // LOCAL INSTANCE // ----- //

    public PlayerFactionSO OurFaction(){
        if(OurInstance == null)
            return null;
        return OurInstance.Faction;
    }

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
            case "ADD BOT":
                to_log = NewBot();
                break;
            case "DESTROY BOT":
                to_log = DestroyBot();
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
        free_factions = new Queue<PlayerFactionSO>(AllFactions);
        foreach(PlayerFactionSO faction in AllFactions)
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

    public void SetInstanceFaction(PlayerInstance instance){
        free_factions.Enqueue(instance.Faction);
        instance.SetFaction(free_factions.Dequeue());
    }

    public string  NewBot(){
        if(!CanAddBots())
            return ErrorWrap("At max bot instance capacity.");

        PlayerInstance new_bot = GameObject.Instantiate(PlayerInstancePrefab).GetComponent<PlayerInstance>();
        new_bot.ID = NewPlayerInstanceID();
        new_bot.Username = $"[Bot #{new_bot.ID}]";
        new_bot.SetFaction(free_factions.Dequeue());
        new_bot._SessionManager = this;
        OtherInstances.Add(new_bot);
        _LobbyController.RegenerateUI();

        return $"{new_bot.Username} created.";
    }

    public string DestroyBot(){
        if(!CanRemoveBots())
            return ErrorWrap("No bots left to destroy.");

        PlayerInstance last_bot = OtherInstances[OtherInstances.Count - 1];
        string bot_name = last_bot.Username;
        free_factions.Enqueue(last_bot.Faction);
        Destroy(last_bot.gameObject);
        OtherInstances.RemoveAt(OtherInstances.Count - 1);
        _LobbyController.RegenerateUI();

        return $"{bot_name} destroyed.";
    }

    private (string error, TerritoryInstance territory, PlayerFactionSO faction) TryGetArgs(string[] commands)
    {
        if (commands.Length < 2)
            return (ErrorWrap("Must denote a territory!"), null, null);

        var territory = GetTerritoryInstance(commands[1]);
        var faction = commands.Length > 2 ? GetFaction(commands[2]) : OurInstance.Faction;

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

    // ----- // --- COMMAND FUNCTIONS --- // ----- //

    // Booleans //

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

    /*
        all of format:
        "boolean" (toggles on/off).
        "boolean.VALUE" - true, false
    */

    string SetAdmin(string[] command){
        ToggleBooleanCommand(ref admin, command);
        return $"Admin mode set to {admin}";
    }

    // Management //

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

    // Getters //

    public bool Admin(){return admin;}
    
    public PlayerInstance GetOtherInstance(int index){
        if (index >= OtherInstances.Count)
            return null;
        return OtherInstances[index];
    }

    public bool CanAddBots(){return OtherInstances.Count < AllFactions.Count - player_count;}
    public bool CanRemoveBots(){return OtherInstances.Count > 0;}
}