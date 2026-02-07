using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RISK_Utils;

public class PlayerInstance : MonoBehaviour
{    
    // This is the object that 'speaks' for the player.
    // This sends commands and holds player information.

    [Header("Identifiers")]
    public string Username;
    public int ID;
    public PlayerFactionSO Faction;

    [Header("References")]
    public SessionManager _SessionManager;

    public void Speak(string message){
        _SessionManager.ProcessCommand(message);
    }

    public void SetFaction(PlayerFactionSO faction){
        Faction = faction;
    }
}
