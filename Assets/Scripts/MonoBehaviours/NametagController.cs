using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NametagController : MonoBehaviour
{
    [SerializeField] TMP_Text NameDisplay;
    [SerializeField] TMP_Text TroopDisplay;

    public void UpdateRotation(float new_rot){
        transform.localEulerAngles = new Vector3(0f, 0f, -new_rot);
    }
    
    public void SetName(string name){
        NameDisplay.text = name;
        gameObject.name = name;
    }

    public void UpdateMode(bool mode){
        NameDisplay.transform.parent.gameObject.SetActive(mode);
        TroopDisplay.transform.parent.gameObject.SetActive(!mode);
    }
}
