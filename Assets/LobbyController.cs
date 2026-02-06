using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    
    [SerializeField] Animator FlagWheel;
    
    [SerializeField] RawImage[] CentreMostFlags;
    [SerializeField] RawImage[] LeftMostFlags;
    [SerializeField] RawImage[] RightMostFlags;

    [SerializeField] PlayerFactionSO[] Factions;
    int faction_pointer, left_pointer, right_pointer;
    bool can_move_flags = true;

    void Start(){
        faction_pointer = 0;
        can_move_flags = true;
        UpdateFlags();
    }

    public void MovePointer(int movement){
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
        UpdatePointers();
        foreach(RawImage image in CentreMostFlags)
            image.texture = Factions[faction_pointer].Flag;
        foreach(RawImage image in LeftMostFlags)
            image.texture = Factions[left_pointer].Flag;
        foreach(RawImage image in RightMostFlags)
            image.texture = Factions[right_pointer].Flag;
    }

    void UpdatePointers(){
        if(faction_pointer < 0)
            faction_pointer = Factions.Length -1;
        if(faction_pointer >= Factions.Length)
            faction_pointer = 0;
        right_pointer = faction_pointer + 1;
        if(right_pointer >= Factions.Length)
            right_pointer = 0;
        left_pointer = faction_pointer - 1;
        if(left_pointer < 0)
            left_pointer = Factions.Length - 1;
    }
}
