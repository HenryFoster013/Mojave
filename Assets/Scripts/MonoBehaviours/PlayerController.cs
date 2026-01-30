using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;
using RISK_Utils;

public class PlayerController : MonoBehaviour
{
    [Header("Internal References")]
    [SerializeField] Camera Cam;
    
    [Header("External References")]
    [SerializeField] MapManager Map;
    [SerializeField] SoundEffectLookupSO SoundEffectLookup;
     
    void Update(){
        ManageKeyboardInputs();
    }

    void ManageKeyboardInputs(){
        if(Input.GetKeyDown("k"))
            Map.FlipRenderMode();
    }

    public void Select(TerritoryInstance territory){
        if(territory == null)
            Deselect();
    }

    public void Deselect(){

    }
}
