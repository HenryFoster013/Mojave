using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;
using RISK_Utils;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TerminalController : MonoBehaviour
{   

    [Header("References")]
    [SerializeField] SessionManager _SessionManager;
    [SerializeField] SoundEffectLookupSO SFX_Lookup;

    [Header("UI")]
    [SerializeField] GameObject Holder;
    [SerializeField] TMP_Text UserTypeDisplay;
    [SerializeField] TMP_InputField InputBox;
    [SerializeField] TMP_Text Log;

    bool activated, second_tap;
    int command_position = -1;

    const string admin_tag = "<color=green>admin~</color>";
    const string user_tag = "<color=orange>user~</color>";

    [HideInInspector]
    public bool input_focused;

    List<string> buffered_commands = new List<string>();

    // Setup //

    void Start(){
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
        UserTypeDisplay.text = UserType();
    }

    // Interaction //

    public bool Focused(){
        return activated && InputBox.isFocused;
    }

    public void LogLine(string to_log){
        Log.text += to_log + "\n";

        if (Log.textInfo.lineCount >= 50){
            string[] lines = Log.text.Split('\n');
            Log.text = string.Join("\n", lines.Skip(lines.Length / 2));
            Log.ForceMeshUpdate();
        }

        UpdateUI();
    }

    public void SubmitInput(){
        if(InputBox.text == "")
            return;

        LogLine(UserType() + InputBox.text);
        buffered_commands.Add(InputBox.text);
        _SessionManager.ProcessCommand(InputBox.text);

        InputBox.text = "";
        second_tap = false;
    }

    string UserType(){
        if(_SessionManager.admin)
            return admin_tag;
        return user_tag;
    }

    void ScrollCommands(bool down){
        if(buffered_commands.Count == 0)
            return;

        if(!second_tap){
            second_tap = true;
            if(down)
                command_position = 0;
            else
                command_position = buffered_commands.Count -1;
        } 
        else{
            if(down)
                command_position++;
            else
                command_position--;
        }

        if(command_position < 0)
            command_position = buffered_commands.Count - 1;
        if(command_position >= buffered_commands.Count)
            command_position = 0;
        
        InputBox.text = buffered_commands[command_position];
    }
}
