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
    [SerializeField] SessionManager _SessionManager;
    [SerializeField] SoundEffectLookupSO SFX_Lookup;

    [Header("UI")]
    [SerializeField] GameObject Holder;
    [SerializeField] TMP_InputField InputBox;
    [SerializeField] TMP_Text Log;

    bool activated, admin;

    [HideInInspector]
    public bool input_focused;

    // Setup //

    void Start(){
        admin = DefaultAdmin;
        Disable();
    }

    public void FlipFlopactivatedisable(){
        PlaySFX("pipboy_tab_1", SFX_Lookup);
        activated = !activated;
        UpdateUI();
    }

    public void Disable(){
        activated = false;
        UpdateUI();
    }

    public void Enable(){
        activated = true;
        UpdateUI();
    }

    // UI //

    void UpdateUI(){
        Holder.SetActive(activated);
    }

    // Interaction //

    void Update(){

        input_focused = activated && InputBox.isFocused;
        if(input_focused)
            return;

        if(Input.GetKeyDown("/"))
            FlipFlopactivatedisable();
    }

    public void LogLine(string to_log){
        Log.text += to_log + "\n";
    }

    public void SubmitInput(){
        if(InputBox.text == "")
            return;
        LogLine(InputBox.text);
        _SessionManager.ProcessCommand(InputBox.text);
        InputBox.text = "";
    }
}
