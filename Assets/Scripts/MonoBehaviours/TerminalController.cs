using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;
using RISK_Utils;
using UnityEngine.UI;
using TMPro;

public class TerminalController : MonoBehaviour
{   
    
    public bool DefaultAdmin;

    [Header("References")]
    [SerializeField] GameManager _GameManager;
    [SerializeField] SoundEffectLookupSO SFX_Lookup;

    [Header("UI")]
    [SerializeField] GameObject Holder;
    [SerializeField] TMP_InputField InputBox;
    [SerializeField] TMP_Text Log;

    bool enabled, admin;

    // Setup //

    void Start(){
        admin = DefaultAdmin;
        Disable();
    }

    public void FlipFlopEnableDisable(){
        enabled = !enabled;
        UpdateUI();
    }

    public void Disable(){
        enabled = false;
        UpdateUI();
    }

    public void Enable(){
        enabled = true;
        UpdateUI();
    }

    // UI //

    void UpdateUI(){
        Holder.SetActive(enabled);
    }

    // Interaction //

    void Update(){
        if(Input.GetKeyDown('/'))
            FlipFlopEnableDisable();
    }

    public void LogLine(string to_log){
        Log.text += to_log + "\n";
    }


}
