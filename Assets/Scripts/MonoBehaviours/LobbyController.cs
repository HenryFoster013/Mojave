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
    [Header("Player List")]
    [SerializeField] InstanceCard[] Cards;
    [SerializeField] GameObject PlusButtonValid;
    [SerializeField] GameObject MinusButtonValid;
    [SerializeField] GameObject PlusButtonInvalid;
    [SerializeField] GameObject MinusButtonInvalid;
    
    // Setup //

    public void RegenerateUI(){
        RefreshInstanceCards();
        RefreshButtons();
    }

    void RefreshInstanceCards(){
        Cards[0].Setup(_SessionManager.OurInstance);
        for (int i = 1; i < Cards.Length; i++){
            Cards[i].gameObject.SetActive(i - 1 < _SessionManager.OtherInstances.Count);
            Cards[i].Setup(_SessionManager.GetOtherInstance(i - 1));
        }
    }

    void RefreshButtons(){
        PlusButtonValid.SetActive(_SessionManager.CanAddBots());
        PlusButtonInvalid.SetActive(!_SessionManager.CanAddBots());
        MinusButtonValid.SetActive(_SessionManager.CanRemoveBots());
        MinusButtonInvalid.SetActive(!_SessionManager.CanRemoveBots());
    }

    public void AddBot(){
        if(!_SessionManager.CanAddBots())
            return;
        _SessionManager.NewBot();
        RefreshInstanceCards();
        RefreshButtons();
    }

    public void RemoveBot(){
        if(!_SessionManager.CanRemoveBots())
            return;
        _SessionManager.DestroyBot();
        RefreshInstanceCards();
        RefreshButtons();
    }

    public void CloseLobby(){

    }
}
