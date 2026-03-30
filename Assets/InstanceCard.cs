using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RISK_Utils;

public class InstanceCard : MonoBehaviour
{
    
    [SerializeField] TMP_Text DisplayText;
    [SerializeField] RawImage FlagDisplay;
    [SerializeField] RawImage ColourDisplay;

    public PlayerInstance ReferencedInstance;

    public void Setup(PlayerInstance reference){
        ReferencedInstance = reference;
        RefreshUI();
    }

    public void RefreshUI(){

        if(ReferencedInstance == null)
            return;

        FlagDisplay.texture = ReferencedInstance.Faction.Flag;
        ColourDisplay.color = ReferencedInstance.Faction.Colour;
        DisplayText.text = $"[{ReferencedInstance.Username}]\n{ReferencedInstance.Faction.Name}";
    }

    public void ChangeFactionButton(){
        print("Called CFB");
    }
}
