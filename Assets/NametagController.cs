using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NametagController : MonoBehaviour
{
    [SerializeField] TMP_Text Display;
    
    public void SetName(string name){
        Display.text = name;
    }
}
