using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyController : MonoBehaviour
{
    
    [Header("Main")]
    [SerializeField] GameObject UI_Holder;
    
    [Header("Display")]
    [SerializeField] RectTransform FactionColumn;
    [SerializeField] GameObject SingleplayerUI;
    [SerializeField] GameObject MultiplayerUI;
    [SerializeField] RawImage FactionDisplay;
    [SerializeField] TMP_Text FactionDescription;
    
    [Header("Flags")]
    [SerializeField] Animator FlagWheel;
    [SerializeField] RawImage[] CentreMostFlags;
    [SerializeField] RawImage[] LeftMostFlags;
    [SerializeField] RawImage[] RightMostFlags;

    int faction_pointer, left_pointer, right_pointer;
    bool can_move_flags = true;
    bool setup = false;
    List<PlayerFactionSO> factions = new List<PlayerFactionSO>();

    void Start(){
        CloseLobby();
    }

    public void CloseLobby(){
        UI_Holder.SetActive(false);
    }

    public void RefreshUIMode(bool multiplayer){
        SingleplayerUI.SetActive(!multiplayer);
        MultiplayerUI.SetActive(multiplayer);
        FactionColumn.anchoredPosition = Vector3.zero;
        if(multiplayer)
            FactionColumn.anchoredPosition = new Vector3(110f, 0f, 0f);
    }

    public void LoadFactions(List<PlayerFactionSO> passed_factions){
        factions = passed_factions;
        faction_pointer = 0;
        can_move_flags = true;
        setup = true;
        UpdateFlags();
        UI_Holder.SetActive(true);
    }

    public void MovePointer(int movement){
        if(!setup)
            return;
        if(!can_move_flags)
            return;

        if(movement < 0)
            FlagWheel.Play("Slide Right");
        else
            FlagWheel.Play("Slide Left");

        StartCoroutine(SpinDelay(movement));
    }

    IEnumerator SpinDelay(int movement){
        can_move_flags = false;
        yield return new WaitForSeconds(1f/6f);
        faction_pointer += movement;
        UpdateFlags();
        yield return new WaitForSeconds(1f/6f);
        can_move_flags = true;
    }

    void UpdateFlags(){
        if(!setup)
            return;
        UpdatePointers();
        foreach(RawImage image in CentreMostFlags)
            image.texture = factions[faction_pointer].Flag;
        foreach(RawImage image in LeftMostFlags)
            image.texture = factions[left_pointer].Flag;
        foreach(RawImage image in RightMostFlags)
            image.texture = factions[right_pointer].Flag;
        FactionDescription.text = factions[faction_pointer].Name + "\n\n" + factions[faction_pointer].Description;
        FactionDisplay.texture = factions[faction_pointer].PreviewImage;
    }

    void UpdatePointers(){
        if(faction_pointer < 0)
            faction_pointer = factions.Count -1;
        if(faction_pointer >= factions.Count)
            faction_pointer = 0;
        right_pointer = faction_pointer + 1;
        if(right_pointer >= factions.Count)
            right_pointer = 0;
        left_pointer = faction_pointer - 1;
        if(left_pointer < 0)
            left_pointer = factions.Count - 1;
    }

    public PlayerFactionSO FetchFaction(){return factions[faction_pointer];}
}
