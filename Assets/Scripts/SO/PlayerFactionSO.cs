using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Faction", menuName = "Players/Faction")]
public class PlayerFactionSO : ScriptableObject{

    [Header("Main")]
    public string ID;
    public string Name;
    public Color Colour = new Color(0f,0f,0f,1f);

    [Header("Flags")]
    public Texture2D FlagFull;
    public Texture2D FlagMini;

}
