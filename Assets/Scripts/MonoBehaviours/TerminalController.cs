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

    bool activated, admin, second_tap;
    int command_position = -1;

    [HideInInspector]
    public bool input_focused;

    List<string> buffered_commands = new List<string>();

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
        if(input_focused){
            if(Input.GetKeyDown(KeyCode.UpArrow))
                ScrollUpCommands();
            if(Input.GetKeyDown(KeyCode.DownArrow))
                ScrollDownCommands();
        }
        else{
            if(Input.GetKeyDown("/"))
                FlipFlopactivatedisable();  
        }
    }

    public void LogLine(string to_log){
        Log.text += to_log + "\n";
    }

    public void SubmitInput(){
        if(InputBox.text == "")
            return;
        LogLine(InputBox.text);
        buffered_commands.Add(InputBox.text);
        _SessionManager.ProcessCommand(InputBox.text);
        InputBox.text = "";
        second_tap = false;
    }

    void ScrollUpCommands(){
        if(buffered_commands.Count == 0)
            return;

        if(!second_tap){
            second_tap = true;
            command_position = buffered_commands.Count -1;
        } 
        else{
            command_position--;
            if(command_position < 0)
                command_position = buffered_commands.Count - 1;
        }
        
        InputBox.text = buffered_commands[command_position];
    }

    void ScrollDownCommands(){
        if(buffered_commands.Count == 0)
            return;
        
        if(!second_tap){
            second_tap = true;
            command_position = 0;
        } 
        else{
            command_position++;
            if(command_position >= buffered_commands.Count)
                command_position = 0;
        }

        InputBox.text = buffered_commands[command_position];
    }
}
