using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyController : MonoBehaviour{

    [Header(" - PRIMARY - ")]
    [SerializeField] SessionManager _SessionManager;
    
    [Header(" - UI - ")]
    [SerializeField] GameObject UI_Holder;
    [SerializeField] InstanceCard[] Cards;
    
    // Setup //

    public void RegenerateUI(){
        RefreshInstanceCards();
    }

    void RefreshInstanceCards(){
        Cards[0].Setup(_SessionManager.OurInstance);
        for (int i = 1; i < Cards.Length; i++){
            Cards[i].gameObject.SetActive(i - 1 < _SessionManager.OtherInstances.Count);
            Cards[i].Setup(_SessionManager.GetOtherInstance(i - 1));
        }
    }

    public void CloseLobby(){

    }
}
