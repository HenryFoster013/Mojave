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
    [SerializeField] SessionManager _SessionManager;
    [SerializeField] TerminalController _TerminalController;
    [SerializeField] SoundEffectLookupSO SFX_Lookup;

    [Header("Modifiers")]
    public PlayerFactionSO Faction;
    public Action ClickAction = Action.Selection;
    public enum Action {Selection, Claim, Admin_Paint}

    [Header("UI")]
    [SerializeField] RawImage OurFlagDisplay;

    [HideInInspector]
    public bool is_typing;

    TerritoryInstance selected_territory;

    // MAIN //
     
    void Update(){
        ManageKeyboardInputs();
        UpdateFactionDisplay();
    }

    // INPUTS //

    void ManageKeyboardInputs(){

        is_typing = _TerminalController.input_focused;
        if(is_typing)
            return;

        if(Input.GetKeyDown("k"))
            _MapManager.FlipRenderMode();
    }

    public void TerritoryClicked(TerritoryInstance territory){
        switch(ClickAction){
            case Action.Selection:
                Select(territory);
                break;
            case Action.Claim:
                Claim(territory);
                break;
            case Action.Admin_Paint:
                Admin_Paint(territory);
                break;
        }
    }

    // SELECTION //

    void Select(TerritoryInstance territory){

        if(ClickAction != Action.Selection)
            return;

        Deselect();

        if(territory == null)
            return;

        selected_territory = territory;
        print(selected_territory.definition.Name);
        selected_territory.Select();
        _MapManager.CheckDirtyInstances();
        PlaySFX("pipboy_select_3", SFX_Lookup);
    }

    void Deselect(){

        if(selected_territory == null || ClickAction != Action.Selection)
            return;

        selected_territory.Deselect();
        selected_territory = null;
        _MapManager.CheckDirtyInstances();
        PlaySFX("pipboy_select_4", SFX_Lookup);
    }

    // UI //

    void UpdateFactionDisplay(){
        OurFlagDisplay.texture = Faction.Flag;
    }

    // COMMANDS //

    void Claim(TerritoryInstance territory){
        if(territory == null)
            return;
        _SessionManager.ProcessCommand(Faction.ID + ".CLAIM." + territory.Name());
        PlaySFX("alert_2", SFX_Lookup);       
    }

    void Admin_Paint(TerritoryInstance territory){
        if(territory == null)
            return;
        _SessionManager.ProcessCommand(Faction.ID + ".PAINT." + territory.Name());
        PlaySFX("keyboard_1", SFX_Lookup);
    }
}
