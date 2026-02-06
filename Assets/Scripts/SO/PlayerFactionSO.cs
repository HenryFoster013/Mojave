using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Faction", menuName = "Players/Faction")]
public class PlayerFactionSO : ScriptableObject{

    [Header("Main")]
    public string ID;
    public string Name;
    [TextArea(15,20)]
    public string Description;
    public Color Colour = new Color(0f,0f,0f,1f);
    public float IncompleteAlpha = 0.8f;

    [Header("Flag")]
    public Texture2D Flag;

}
