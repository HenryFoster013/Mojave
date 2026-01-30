using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;
using RISK_Utils;

public class PlayerController : MonoBehaviour
{    
    [Header("Primary References")]
    [SerializeField] MapManager _MapManager;
    [SerializeField] SoundEffectLookupSO SoundEffectLookup;

    TerritoryInstance selected_territory;
     
    void Update(){
        ManageKeyboardInputs();
    }

    void ManageKeyboardInputs(){
        if(Input.GetKeyDown("k"))
            _MapManager.FlipRenderMode();
    }

    public void Select(TerritoryInstance territory){
        
        Deselect();

        if(territory == null)
            return;

        selected_territory = territory;

        print(selected_territory.definition.Name);
        selected_territory.Select();
        _MapManager.CheckDirtyInstances();
    }

    public void Deselect(){
        
        if(selected_territory == null)
            return;

        selected_territory.Deselect();
        _MapManager.CheckDirtyInstances();
    }
}
