using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;

public class PlayerController : MonoBehaviour
{
    [SerializeField] MapManager Map;
    [SerializeField] SoundEffectLookupSO SoundEffectLookup;
     
    void Update(){
        ManageKeyboardInputs();
    }

    void ManageKeyboardInputs(){
        if(Input.GetKeyDown("k"))
            Map.FlipRenderMode();
    }
}
