using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RISK_Utils;

public class InstanceCard : MonoBehaviour
{
    
    [SerializeField] TMP_Text UsernameText;
    [SerializeField] TMP_Text FactionText;
    [SerializeField] RawImage FlagDisplay;
    [SerializeField] RawImage ColourDisplay;

    public PlayerInstance ReferencedInstance;
    [SerializeField] SessionManager _SessionManager;

    public void Setup(PlayerInstance reference){
        ReferencedInstance = reference;
        RefreshUI();
    }

    public void RefreshUI(){

        if(ReferencedInstance == null)
            return;

        FlagDisplay.texture = ReferencedInstance.Faction.Flag;
        ColourDisplay.color = ReferencedInstance.Faction.Colour;
        UsernameText.text = $"[{ReferencedInstance.Username}]";
        FactionText.text = ReferencedInstance.Faction.Name;
    }

    public void ChangeFactionButton(){
        _SessionManager.SetInstanceFaction(ReferencedInstance);
        RefreshUI();
    }
}
